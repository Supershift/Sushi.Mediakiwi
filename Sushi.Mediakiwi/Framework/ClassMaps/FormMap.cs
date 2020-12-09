﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Framework;
using static Sushi.Mediakiwi.Beta.GeneratedCms.Source.Component;

namespace Sushi.Mediakiwi.Framework
{
    public class StateForm : FormMap<ComponentListTemplate>
    {
        public StateForm(ComponentListTemplate list)
        {
            Load(list);
            Map(x => x.FormState).TextField("State").Cloak(true);
        }
    }

    public class FormMap
    {
    }

    public class FormMapList : IFormMap
    {
        public FormMapList()
        {
            Elements = new List<IContentInfo>();
        }

        public ContentInfo Map<Y>(Expression<Func<Y, object>> memberExpression, Y sender)
        {
            PropertyInfo property = ReflectionHelper.GetMember(memberExpression.Body);
            var result = new ContentInfo(property, IsHidden, IsReadOnly, IsCloacked, Elements);
            result.SenderInstance = sender;
            return result;
        }

        public void Evaluate()
        {
        }

        public void Init(WimComponentListRoot wim)
        {
            if (this.Elements == null) return;

            foreach (var element in this.Elements)
            {
                //  SET IT FROM POSTBACK INSTANT!
                if (element.Property != null)
                {
                    element.InfoItem = new ListInfoItem();
                    element.InfoItem.ContentAttribute = element;
                    element.InfoItem.Info = element.Property;

                    if (element.HasSenderInstance)
                        element.InfoItem.SenderInstance = element.SenderInstance;
                    else
                        element.InfoItem.SenderInstance = SenderInstance;

                    element.InfoItem.SenderSponsorInstance = this;
                    if (string.IsNullOrWhiteSpace(UniqueId))
                        element.InfoItem.ContentAttribute.ID = element.Property.Name;
                    else
                        element.InfoItem.ContentAttribute.ID = $"{UniqueId}_{element.Property.Name}";

                    //  Set
                    if (element is IContentInfo)
                    {
                        if (wim.Console != null)
                        {
                            ((IContentInfo)element).Init(wim);
                            if (wim.Console.IsPosted(element.InfoItem.ContentAttribute.ID))
                                ((IContentInfo)element).SetCandidate(wim.IsEditMode);
                            ((IContentInfo)element).Chain(element.InfoItem.ContentAttribute.ID);
                        }
                    }
                }
            }
        }
        public List<IContentInfo> Elements { get; set; }
        public string UniqueId { get; set; }
        List<IFormMap> IFormMap.FormMaps { get; set; }
        public object SenderInstance { get;set; }
        public bool? IsHidden { get; set; }
        public bool? IsCloacked { get; set; }
        public bool? IsReadOnly { get; set; }
    }

    public class FormMap<T> : FormMap, IFormMap
    {
        public ContentInfo Map<Y>(Expression<Func<Y, object>> memberExpression, Y sender)
        {
            PropertyInfo property = ReflectionHelper.GetMember(memberExpression.Body);
            var result = new ContentInfo(property, IsHidden, IsReadOnly, IsCloacked, Elements);
            result.SenderInstance = sender;
            return result;
        }

        public List<IFormMap> FormMaps { get; set; }
        public WimComponentListRoot wim { get; set; }

        public void Init(WimComponentListRoot wim)
        {
            this.wim = wim;
            if (this.Elements == null) return;

            foreach (var element in this.Elements)
            {
                //  SET IT FROM POSTBACK INSTANT!
                if (element.Property != null)
                {
                    element.InfoItem = new ListInfoItem();
                    element.InfoItem.ContentAttribute = element;
                    element.InfoItem.Info = element.Property;

                    if (element.HasSenderInstance)
                        element.InfoItem.SenderInstance = element.SenderInstance; 
                    else
                        element.InfoItem.SenderInstance = SenderInstance;

                    element.InfoItem.SenderSponsorInstance = this;
                    if (string.IsNullOrWhiteSpace(UniqueId))
                        element.InfoItem.ContentAttribute.ID = element.Property.Name;
                    else
                        element.InfoItem.ContentAttribute.ID = $"{UniqueId}_{element.Property.Name}";

                    //  Set
                    if (element is IContentInfo)
                    {
                        if (wim.Console != null)
                        {
                            ((IContentInfo)element).Init(wim);
                            if (wim.Console.IsPosted(element.InfoItem.ContentAttribute.ID))
                                ((IContentInfo)element).SetCandidate(wim.IsEditMode);
                            ((IContentInfo)element).Chain(element.InfoItem.ContentAttribute.ID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is always called, work simular to PreRender Event and can be overriden
        /// </summary>
        public virtual void Evaluate()
        {

        }

        public FormMap()
        {
            Elements = new List<IContentInfo>();
        }

        internal Type EntityType
        {
            get { return typeof(T); }
        }

        public T CreateInstance()
        {
            return System.Activator.CreateInstance<T>();
        }

        public bool? IsHidden { get; set; }
        public bool? IsCloacked { get; set; }
        public bool? IsReadOnly { get; set; }

        public void Load(T entity, object uniqueId = null)
        {
            if (entity == null)
                entity = CreateInstance();

            Instance = entity;
            SenderInstance = entity;

            if (uniqueId != null)
                UniqueId = uniqueId.ToString();
        }

        public object SenderInstance { get; set; }
        public T Instance { get; set; }

        public List<IContentInfo> Elements { get; set; }

        public string UniqueId { get; set; }

        public void Id(object uniqueId)
        {
            UniqueId = uniqueId.ToString();
        }

        public void MapHeader(string title)
        {
            var result = new ContentInfo(null, IsHidden, IsReadOnly, IsCloacked, Elements);
            result.Section(title);
        }

        public ContentInfo Map(Expression<Func<T, object>> memberExpression)
        {
            PropertyInfo property = ReflectionHelper.GetMember(memberExpression.Body);
            var result = new ContentInfo(property, IsHidden, IsReadOnly, IsCloacked, Elements);
            return result;
        }

        public ContentSettings Find(Expression<Func<T, object>> memberExpression)
        {
            PropertyInfo property = ReflectionHelper.GetMember(memberExpression.Body);

            var find = Elements.Where(x => property == x.Property).FirstOrDefault();

            if (find == null) return null;
            ContentSettings setting = new ContentSettings(find);
            return setting;
        }
    }
}
