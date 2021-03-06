using QlikView.Qvx.QvxLibrary;

namespace QvCSGOConnector {
    internal class QvCSGOServer : QvxServer {
        public override QvxConnection CreateConnection() { 
            return new QvCSGOConnection(); 
        }
        public override string CreateConnectionString() {
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Debug, "CreateConnectionString()");
            return "Server=localhost";
        }
        public override string HandleJsonRequest(string method, string[] userParameters, QvxConnection connection)
        {
            QvDataContractResponse response;

            /**
             * -- How to get hold of connection details? --
             *
             * Provider, username and password are always available in
             * connection.MParameters if they exist in the connection
             * stored in the QlikView Repository Service (QRS).
             *
             * If there are any other user/connector defined parameters in the
             * connection string they can be retrieved in the same way as seen
             * below
             */

            string provider, host, username, password, directory;
            connection.MParameters.TryGetValue("provider", out provider); // Set to the name of the connector by QlikView Engine
            connection.MParameters.TryGetValue("userid", out username); // Set when creating new connection or from inside the QlikView Management Console (QMC)
            connection.MParameters.TryGetValue("password", out password); // Same as for username
            connection.MParameters.TryGetValue("host", out host); // Defined when calling createNewConnection in connectdialog.js
            connection.MParameters.TryGetValue("directory", out directory);

            switch (method)
            {
                case "getInfo":
                    response = getInfo();
                    break;
                case "getDatabases":
                    response = getDatabases(username, password);
                    break;
                case "getTables":
                    response = getTables(username, password, connection, userParameters[0], userParameters[1]);
                    break;
                case "getFields":
                    response = getFields(username, password, connection, userParameters[0], userParameters[1], userParameters[2]);
                    break;
                case "testConnection":
                    response = testConnection(userParameters[0], userParameters[1]);
                    break;
                default:
                    response = new Info { qMessage = "Unknown command" };
                    break;
            }
            return ToJson(response);    // serializes response into JSON string
        }
        public bool verifyCredentials (string username, string password) {
            //return (username == "sdk-user" && password == "sdk-password") || (username == "try" && password == "me");
            return true;
        }

        public QvDataContractResponse getInfo()
        {
            return new Info
            {
                qMessage = "Example connector for Windows Event Log. Use account sdk-user/sdk-password"
            };
        }

        public QvDataContractResponse getDatabases(string username, string password)
        {
            if (verifyCredentials(username, password))
            {
                return new QvDataContractDatabaseListResponse
                {
                    qDatabases = new Database[]
                    {
                        new Database {qName = "Demo Files"}
                    }
                };
            }
            return new Info { qMessage = "Credentials WRONG!" };
        }

        public QvDataContractResponse getTables(string username, string password, QvxConnection connection, string database, string owner)
        {
            if (verifyCredentials(username, password))
            {
                return new QvDataContractTableListResponse
                {
                    qTables = connection.MTables
                };
            }
            return new Info { qMessage = "Credentials WRONG!" };
        }

        public QvDataContractResponse getFields(string username, string password, QvxConnection connection, string database, string owner, string table)
        {
            if (verifyCredentials(username, password))
            {
                var currentTable = connection.FindTable(table, connection.MTables);

                return new QvDataContractFieldListResponse
                {
                    qFields = (currentTable != null) ? currentTable.Fields : new QvxField[0]
                };
            }
            return new Info { qMessage = "Credentials WRONG!" };
        }

        public QvDataContractResponse testConnection(string username, string password)
        {
            var message = "Credentials WRONG!";

            if (verifyCredentials( username, password ) ) {
                message = "Credentials OK!";
            }
            return new Info { qMessage = message };
        }
    }
}