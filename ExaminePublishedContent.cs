using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.ExamineDevStuff {
    
    public class ExaminePublishedContent : IPublishedContent {

        private readonly List<IPublishedProperty> _properties = new List<IPublishedProperty>();

        public ExaminePublishedContent(SearchResult result) {

            if (result == null) throw new ArgumentNullException("result");

            DocumentTypeAlias = result.Fields["nodeTypeAlias"];
            Id = result.Id;
            ContentType = PublishedContentType.Get(PublishedItemType.Content, DocumentTypeAlias);

            foreach (KeyValuePair<string, string> field in result.Fields) {

                switch (field.Key) {

                    case "parentID": /* IPublishedContent doesn't have a property for the parent ID */ break;

                    case "level": Level = Int32.Parse(field.Value); break;

                    case "writerID": WriterId = Int32.Parse(field.Value); break;
                    case "writerName": WriterName = field.Value; break;

                    case "creatorID": CreatorId = Int32.Parse(field.Value); break;
                    case "creatorName": CreatorName = field.Value; break;

                    case "nodeType": DocumentTypeId = Int32.Parse(field.Value); break;
                    case "template": TemplateId = Int32.Parse(field.Value); break;
                    case "sortOrder": SortOrder = Int32.Parse(field.Value); break;

                    case "createDate": CreateDate = DateTime.ParseExact(field.Value.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.CurrentCulture); break;
                    case "updateDate": UpdateDate = DateTime.ParseExact(field.Value.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.CurrentCulture); break;

                    case "nodeName": Name = field.Value; break;
                    case "urlName": UrlName = field.Value; break;

                    case "__Path": Path = field.Value; break;

                }

                // Get the type of the field/property (not all fields is a property)
                PublishedPropertyType type = ContentType.GetPropertyType(field.Key);
                if (type == null) continue;

                // Append the property to the list of properties
                _properties.Add(new ExaminePublishedProperty(type, field.Value));

            }

        }

        public int GetIndex() {
            throw new NotImplementedException();
        }

        public IPublishedProperty GetProperty(string alias) {
            return _properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        public IPublishedProperty GetProperty(string alias, bool recurse) {
            if (recurse) throw new NotSupportedException();
            return GetProperty(alias);
        }

        public IEnumerable<IPublishedContent> ContentSet { get; private set; }
        public PublishedContentType ContentType { get; private set; }
        public int Id { get; private set; }
        public int TemplateId { get; private set; }
        public int SortOrder { get; private set; }
        public string Name { get; private set; }
        public string UrlName { get; private set; }
        public string DocumentTypeAlias { get; private set; }
        public int DocumentTypeId { get; private set; }
        public string WriterName { get; private set; }
        public string CreatorName { get; private set; }
        public int WriterId { get; private set; }
        public int CreatorId { get; private set; }
        public string Path { get; private set; }
        public DateTime CreateDate { get; private set; }
        public DateTime UpdateDate { get; private set; }
        public Guid Version { get; private set; }
        public int Level { get; private set; }
        
        public string Url {
            get { throw new NotImplementedException(); }
        }
        
        public PublishedItemType ItemType { get; private set; }

        public bool IsDraft {
            get { return false; }
        }
        
        public IPublishedContent Parent {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IPublishedContent> Children {
            get { return new IPublishedContent[0]; }
        }

        public ICollection<IPublishedProperty> Properties {
            get { return _properties; }
        }

        public object this[string alias] {
            get { throw new NotImplementedException(); }
        }

        public T Select<T>(Func<IPublishedContent, T> func) {
            return func(this);
        }

    }

}