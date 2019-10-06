using QlikView.Qvx.QvxLibrary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace QvCSGOConnector {
    public class QvCSGOConnection : QvxConnection {
        public override void Init() {
            var eventLogFields = new QvxField[] {
                new QvxField("Category", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("EntryType", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("Message", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII)
            };

            MTables = new List<QvxTable> {
                new QvxTable {
                    TableName = "ApplicationsEventLog",
                    GetRows = GetApplicationEvents,
                    Fields = eventLogFields
                }
            };
        }

        private IEnumerable<QvxDataRow> GetApplicationEvents() {
            var ev = new EventLog("Application");
            
            foreach (var ev1 in ev.Entries) {
                yield return MakeEntry(ev1 as EventLogEntry, FindTable("ApplicationsEventLog", MTables));
            }
        }

        private QvxDataRow MakeEntry(EventLogEntry ev1, QvxTable table) {
            var row = new QvxDataRow();
            row[table.Fields[0]] = ev1.Category;
            row[table.Fields[1]] = ev1.EntryType.ToString();
            row[table.Fields[2]] = ev1.Message;
            return row;
        }

        public override QvxDataTable ExtractQuery(string query, List<QvxTable> qvxTables)
        {
            /* Make sure to remove your quotesuffix, quoteprefix, 
             * quotesuffixfordoublequotes, quoteprefixfordoublequotes
             * as defined in selectdialog.js somewhere around here.
             * 
             * In this example it is an escaped double quote that is
             * the quoteprefix/suffix
             */
            query = Regex.Replace(query, "\\\"", "");

            return base.ExtractQuery(query, qvxTables);
        }
    }
}