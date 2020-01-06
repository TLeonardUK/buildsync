using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace BuildSync.Core.Utils
{
    // Based on https://www.codeproject.com/Articles/9280/Add-Remove-Items-to-from-PropertyGrid-at-Runtime

    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyGridProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TypeConverter Converter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InValue"></param>
        /// <param name="InReadOnly"></param>
        /// <param name="InVisible"></param>
        public DynamicPropertyGridProperty(string InName, string InDescription, string InCategory, object InValue, object InDefaultValue, bool InReadOnly, bool InVisible)
        {
            Name = InName;
            Description = InDescription;
            Category = InCategory;
            Value = InValue;
            DefaultValue = InDefaultValue;
            ReadOnly = InReadOnly;
            Visible = InVisible;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyGridRangedProperty : DynamicPropertyGridProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public float MinValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override object Value 
        { 
            get 
            {
                return base.Value;
            }
            set
            {
                object NewValue = value;
                if (value is int)
                {
                    NewValue = Math.Max(Math.Min((int)NewValue, (int)MaxValue), (int)MinValue);
                }
                else if (value is float)
                {
                    NewValue = Math.Max(Math.Min((float)NewValue, (float)MaxValue), (float)MinValue);
                }
                base.Value = NewValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InValue"></param>
        /// <param name="InReadOnly"></param>
        /// <param name="InVisible"></param>
        public DynamicPropertyGridRangedProperty(string InName, string InDescription, string InCategory, object InValue, object InDefaultValue, bool InReadOnly, bool InVisible, float InMinValue, float InMaxValue)
            : base(InName, InDescription, InCategory, InValue, InDefaultValue, InReadOnly, InVisible)
        {
            MinValue = InMinValue;
            MaxValue = InMaxValue;
            Value = InValue;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyGridOptionsProperty : DynamicPropertyGridProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InValue"></param>
        /// <param name="InReadOnly"></param>
        /// <param name="InVisible"></param>
        public DynamicPropertyGridOptionsProperty(string InName, string InDescription, string InCategory, object InValue, object InDefaultValue, bool InReadOnly, bool InVisible, List<string> InOptions)
            : base(InName, InDescription, InCategory, InValue, InDefaultValue, InReadOnly, InVisible)
        {
            Options = InOptions;
            Value = InValue;

            Converter = new DynamicPropertyGridOptionsTypeConverter() { Options = this.Options };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyGridOptionsTypeConverter : StringConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Options);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyGridPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        DynamicPropertyGridProperty Property;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InProperty"></param>
        /// <param name="InAttributes"></param>
        public DynamicPropertyGridPropertyDescriptor(ref DynamicPropertyGridProperty InProperty, Attribute[] InAttributes)
            : base(InProperty.Name, InAttributes)
        {
            Property = InProperty;
        }

        #region PropertyDescriptor Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            return Property.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Description
        {
            get
            {
                return Property.Description;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Category
        {
            get
            {
                return Property.Category;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return Property.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return Property.ReadOnly;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
            Property.Value = Property.DefaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            Property.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Type PropertyType
        {
            get { return Property.Value.GetType(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override TypeConverter Converter
        {
            get 
            {
                if (Property.Converter != null)
                {
                    return Property.Converter;
                }
                return base.Converter;
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyGridObject : CollectionBase, ICustomTypeDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void Add(DynamicPropertyGridProperty Value)
        {
            base.List.Add(Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Name)
        {
            foreach (DynamicPropertyGridProperty prop in base.List)
            {
                if (prop.Name == Name)
                {
                    base.List.Remove(prop);
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public DynamicPropertyGridProperty Find(string Name)
        {
            foreach (DynamicPropertyGridProperty prop in base.List)
            {
                if (prop.Name == Name)
                {
                    return prop;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DynamicPropertyGridProperty this[int index]
        {
            get
            {
                return (DynamicPropertyGridProperty)base.List[index];
            }
            set
            {
                base.List[index] = (DynamicPropertyGridProperty)value;
            }
        }

        #region TypeDescriptor Implementation

        /// <summary>
        /// Get Class Name
        /// </summary>
        /// <returns>String</returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <returns>AttributeCollection</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// GetComponentName
        /// </summary>
        /// <returns>String</returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// GetConverter
        /// </summary>
        /// <returns>TypeConverter</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// GetDefaultEvent
        /// </summary>
        /// <returns>EventDescriptor</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// GetDefaultProperty
        /// </summary>
        /// <returns>PropertyDescriptor</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// GetEditor
        /// </summary>
        /// <param name="editorBaseType">editorBaseType</param>
        /// <returns>object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] Attributes)
        {
            PropertyDescriptor[] NewProperties = new PropertyDescriptor[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                DynamicPropertyGridProperty prop = (DynamicPropertyGridProperty)this[i];
                if (prop.Name.Length > 0)
                {
                    NewProperties[i] = new DynamicPropertyGridPropertyDescriptor(ref prop, Attributes);
                }
            }
            return new PropertyDescriptorCollection(NewProperties);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
