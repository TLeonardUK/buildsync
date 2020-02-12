/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace BuildSync.Core.Utils
{
    // Based on https://www.codeproject.com/Articles/9280/Add-Remove-Items-to-from-PropertyGrid-at-Runtime

    /// <summary>
    /// </summary>
    public class DynamicPropertyGridProperty
    {
        /// <summary>
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// </summary>
        public TypeConverter Converter { get; set; }

        /// <summary>
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InValue"></param>
        /// <param name="InReadOnly"></param>
        /// <param name="InVisible"></param>
        public DynamicPropertyGridProperty(string InId, string InName, string InDescription, string InCategory, object InValue, object InDefaultValue, bool InReadOnly, bool InVisible)
        {
            Id = InId;
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
    /// </summary>
    public class DynamicPropertyGridRangedProperty : DynamicPropertyGridProperty
    {
        /// <summary>
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// </summary>
        public float MinValue { get; set; }

        /// <summary>
        /// </summary>
        public override object Value
        {
            get => base.Value;
            set
            {
                object NewValue = value;
                if (value is int)
                {
                    NewValue = Math.Max(Math.Min((int) NewValue, (int) MaxValue), (int) MinValue);
                }
                else if (value is float)
                {
                    NewValue = Math.Max(Math.Min((float) NewValue, MaxValue), MinValue);
                }

                base.Value = NewValue;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InValue"></param>
        /// <param name="InReadOnly"></param>
        /// <param name="InVisible"></param>
        public DynamicPropertyGridRangedProperty(string InId, string InName, string InDescription, string InCategory, object InValue, object InDefaultValue, bool InReadOnly, bool InVisible, float InMinValue, float InMaxValue)
            : base(InId, InName, InDescription, InCategory, InValue, InDefaultValue, InReadOnly, InVisible)
        {
            MinValue = InMinValue;
            MaxValue = InMaxValue;
            Value = InValue;
        }
    }

    /// <summary>
    /// </summary>
    public class DynamicPropertyGridOptionsProperty : DynamicPropertyGridProperty
    {
        /// <summary>
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// </summary>
        /// <param name="InName"></param>
        /// <param name="InValue"></param>
        /// <param name="InReadOnly"></param>
        /// <param name="InVisible"></param>
        public DynamicPropertyGridOptionsProperty(string InId, string InName, string InDescription, string InCategory, object InValue, object InDefaultValue, bool InReadOnly, bool InVisible, List<string> InOptions)
            : base(InId, InName, InDescription, InCategory, InValue, InDefaultValue, InReadOnly, InVisible)
        {
            Options = InOptions;
            Value = InValue;

            Converter = new DynamicPropertyGridOptionsTypeConverter {Options = Options};
        }
    }

    /// <summary>
    /// </summary>
    public class DynamicPropertyGridOptionsTypeConverter : StringConverter
    {
        /// <summary>
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Options);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    /// <summary>
    /// </summary>
    public class DynamicPropertyGridPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// </summary>
        private readonly DynamicPropertyGridProperty Property;

        /// <summary>
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
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// </summary>
        public override Type ComponentType => null;

        /// <summary>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            return Property.Value;
        }

        /// <summary>
        /// </summary>
        public override string Description => Property.Description;

        /// <summary>
        /// </summary>
        public override string Category => Property.Category;

        /// <summary>
        /// </summary>
        public override string DisplayName => Property.Name;

        /// <summary>
        /// </summary>
        public override bool IsReadOnly => Property.ReadOnly;

        /// <summary>
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
            Property.Value = Property.DefaultValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            Property.Value = value;
        }

        /// <summary>
        /// </summary>
        public override Type PropertyType => Property.Value.GetType();

        /// <summary>
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
    /// </summary>
    public class DynamicPropertyGridObject : CollectionBase, ICustomTypeDescriptor
    {
        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DynamicPropertyGridProperty this[int index]
        {
            get => (DynamicPropertyGridProperty) List[index];
            set => List[index] = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="Value"></param>
        public void Add(DynamicPropertyGridProperty Value)
        {
            List.Add(Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        public DynamicPropertyGridProperty Find(string Id)
        {
            foreach (DynamicPropertyGridProperty prop in List)
            {
                if (prop.Id == Id)
                {
                    return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Id)
        {
            foreach (DynamicPropertyGridProperty prop in List)
            {
                if (prop.Id == Id)
                {
                    List.Remove(prop);
                    return;
                }
            }
        }

        #region TypeDescriptor Implementation

        /// <summary>
        ///     Get Class Name
        /// </summary>
        /// <returns>String</returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        ///     GetAttributes
        /// </summary>
        /// <returns>AttributeCollection</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        ///     GetComponentName
        /// </summary>
        /// <returns>String</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        ///     GetConverter
        /// </summary>
        /// <returns>TypeConverter</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        ///     GetDefaultEvent
        /// </summary>
        /// <returns>EventDescriptor</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        ///     GetDefaultProperty
        /// </summary>
        /// <returns>PropertyDescriptor</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        ///     GetEditor
        /// </summary>
        /// <param name="editorBaseType">editorBaseType</param>
        /// <returns>object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// </summary>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] Attributes)
        {
            PropertyDescriptor[] NewProperties = new PropertyDescriptor[Count];
            for (int i = 0; i < Count; i++)
            {
                DynamicPropertyGridProperty prop = this[i];
                if (prop.Name.Length > 0)
                {
                    NewProperties[i] = new DynamicPropertyGridPropertyDescriptor(ref prop, Attributes);
                }
            }

            return new PropertyDescriptorCollection(NewProperties);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        /// <summary>
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