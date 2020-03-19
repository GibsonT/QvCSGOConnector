using QlikView.Qvx.QvxLibrary;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace QvCSGOConnector {
    public class QvCSGOConnection : QvxConnection {
        public override void Init() {
            var killFields = new QvxField[] {
                new QvxField("KillerName", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("AssisterName", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("VictimName", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("Weapon", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("PenetratedObjects", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                new QvxField("Headshot", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII)
            };

            MTables = new List<QvxTable> {
                new QvxTable {
                    TableName = "Kills",
                    GetRows = GetKills,
                    Fields = killFields
                }
            };
        }

        private IEnumerable<QvxDataRow> GetKills() {
            string directory;
            this.MParameters.TryGetValue("directory", out directory);
            var files = Directory.GetFiles(directory, "*.dem", SearchOption.AllDirectories);

            foreach (var file in files) {
                var proc = new Process() {
                    StartInfo = new ProcessStartInfo() {
                        FileName = "parser.exe",
                        Arguments = "-demoPath \"" + file + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, "Parsing demo " + file);
                while (!proc.StandardOutput.EndOfStream) {
                    var rawLine = proc.StandardOutput.ReadLine();
                    var line = rawLine.Split('\t');
                    var row = new QvxDataRow();
                    var table = FindTable("Kills", MTables);

                    row[table.Fields[0]] = line[0];
                    row[table.Fields[1]] = line[1];
                    row[table.Fields[2]] = line[2];
                    row[table.Fields[3]] = line[3];
                    row[table.Fields[4]] = line[4];
                    row[table.Fields[5]] = line[5];

                    QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, "Line info: " + rawLine);
                    yield return row;
                }
            }

            /*foreach (var ev1 in ev.Entries) {
                yield return MakeEntry(ev1 as EventLogEntry, FindTable("Kills", MTables));
            }*/
        }

        /*private QvxDataRow MakeEntry(EventLogEntry ev1, QvxTable table) {
            var row = new QvxDataRow();
            row[table.Fields[0]] = ev1.Category;
            row[table.Fields[1]] = ev1.EntryType.ToString();
            row[table.Fields[2]] = ev1.Message;
            return row;
        }*/

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