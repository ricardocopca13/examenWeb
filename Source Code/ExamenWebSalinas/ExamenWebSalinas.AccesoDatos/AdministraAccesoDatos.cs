using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Vitamedica.Herramientas.AccesoDatos.Entidades;

namespace ExamenWebSalinas.AccesoDatos
{
    public class AdministraAccesoDatos: IDisposable
    {
        
        /// <summary>
        /// Objeto que contiene la conexión a la base de datos.
        /// </summary>
        protected DbConnection conexion;

        /// <summary>
        /// Llave del registry que contiene la dirección del archivo de configuración.
        /// </summary>
        private string dbKey;

        /// <summary>
        /// Provider de la base de datos a la que se conectará.
        /// </summary>
        private TipoProveedor tipoProveedorBD;

        /// <summary>
        /// Constructor del objeto.
        /// </summary>
        /// <param name="claveConfiguracionBD">Dirección dónde se encuentra el archivo de configuración.</param>
        /// <param name="proveedor">Proveedor del manejador de la base de datos (SQL Server u Oracle).</param>
        public AdministraAccesoDatos(string claveConfiguracionBD, TipoProveedor proveedor)
        {
            if (string.IsNullOrEmpty(claveConfiguracionBD.Trim()))
            {
                throw new ArgumentException("No se especificó la llave de configuracion", "string connectionStrings");
            }

            this.dbKey = claveConfiguracionBD;
            this.tipoProveedorBD = proveedor;

            GeneraConexion();
        }

        /// <summary>
        /// Constructor del objeto.
        /// </summary>
        /// <param name="claveConfiguracionBD">Dirección dónde se encuentra el archivo de configuración.</param>
        /// <param name="proveedor">Proveedor del manejador de la base de datos (SQL Server u Oracle).</param>
        public AdministraAccesoDatos(string dbKey = "Default")
        {

            this.dbKey = dbKey;
            this.tipoProveedorBD =  TipoProveedor.SqlServer;

            GeneraConexion();
            conexion.Open();
        }


        /// <summary>
        /// Recupera el objeto de conexión.
        /// </summary>
        /// <returns>Objeto de conexión a la base de datos.</returns>
        public DbConnection ObtenerConexion()
        {
            return this.conexion;
        }

        /// <summary>
        /// Recupera un valor de la base de datos de configuración.
        /// </summary>
        /// <param name="key">Llave de configuración.</param>
        /// <returns>Valor de configuración.</returns>
        private string ObtenerConfiguracion(string key)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// Asigna el string de conexión.
        /// </summary>
        private void GeneraConexion()
        {
            string cadenaConexion = ObtenerConfiguracion(this.dbKey);
            DbProviderFactory factory = DbProviderFactories.GetFactory(ObtenerNombreProveedor());
            this.conexion = factory.CreateConnection();
            this.conexion.ConnectionString = cadenaConexion;
        }

        /// <summary>
        /// Recupera el provider para la base de datos.
        /// </summary>
        /// <returns>Nombre del provider (SQL Server u Oracle).</returns>
        private string ObtenerNombreProveedor()
        {
            string nombreProveedor = "System.Data.SqlClient";

            switch (this.tipoProveedorBD)
            {
                case TipoProveedor.Oracle:
                    nombreProveedor = "System.Data.OracleClient";
                    break;
            }

            return nombreProveedor;
        }

        /// <summary>
        /// Cierra la conexión a la base de datos.
        /// </summary>
        public void CerrarConexion()
        {
            if (this.conexion != null)
            {
                this.conexion.Close();
                this.conexion = null;
            }
        }

        /// <summary>
        /// Crea un objeto command.
        /// </summary>
        /// <param name="commandText">Nombre del Stored Procedure o sentencia SQL a ejecutar.</param>
        /// <param name="commandType">tipo de comando a ejecutar.</param>
        /// <param name="parameters">Lista de parámetros.</param>
        /// <returns>Objeto command.</returns>
        public IDbCommand PrepararComando(string commandText, CommandType commandType, IDataParameter[] parameters)
        {
            IDbCommand comando = conexion.CreateCommand();
            comando.CommandText = commandText;
            comando.CommandType = commandType;

            if (parameters != null)
            {
                foreach (IDataParameter paremetro in parameters)
                {
                    comando.Parameters.Add(paremetro);
                }
            }

            return comando;
        }

        /// <summary>
        /// Recupera datos de una base de datos con base en una sentencia SQL.
        /// </summary>
        /// <param name="sql">Sentencia SQL.</param>
        /// <param name="tipo">Tipo de comando a ejecutar.</param>
        /// <param name="parametros">Lista de parámetros.</param>
        /// <returns>Objeto con los datos recuperados.</returns>
        public IDataReader EjecutaReaderSQL(string sql, CommandType tipo, IDataParameter[] parametros)
        {
            if (string.IsNullOrEmpty(sql.Trim()))
            {
                throw new ArgumentException("No se especificó el comando a ejecutar", "string sql");
            }

            IDataReader reader = null;
            IDbCommand comando;
            try
            {
                this.conexion.Open();
                comando = PrepararComando(sql, tipo, parametros);
                reader = comando.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception sqlEx)
            {
                throw new Exception(sqlEx.Message, sqlEx);
            }

            return reader;
        }

        /// <summary>
        /// Ejecuta una sentencia SQL que recupera un solo dato.
        /// </summary>
        /// <param name="sql">Sentencia SQL.</param>
        /// <param name="parametros">Lista de Parámetros.</param>
        /// <returns>Objeto resultado de la ejecución.</returns>
        public object EjecutarEscalar(string sql, IDataParameter[] parametros)
        {
            if (string.IsNullOrEmpty(sql.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string sql");
            }

            object res = null;
            IDbCommand comando = null;
            try
            {
                this.conexion.Open();
                comando = PrepararComando(sql, CommandType.StoredProcedure, parametros);
                res = comando.ExecuteScalar();
            }
            catch (Exception sqlEx)
            {
                throw new Exception(sqlEx.Message, sqlEx);
            }
            finally
            {
                comando.Dispose();
                comando = null;

                if (this.conexion.State == ConnectionState.Open)
                {
                    this.conexion.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado.
        /// </summary>
        /// <param name="textoComando">Nombre del Procedimiento Almacenado.</param>
        /// <returns>Objeto comando.</returns>
        public object EjecutarReaderStoredProcedure(string textoComando)
        {
            if (string.IsNullOrEmpty(textoComando.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string textoComando");
            }

            IDataReader reader = null;
            IDbCommand comando = null;
            try
            {
                this.conexion.Open();

                comando = PrepararComando(textoComando, CommandType.StoredProcedure, null);

                comando.CommandTimeout = 0;
                reader = comando.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                comando.Dispose();
                comando = null;
            }
            return reader;
        }


        public object EjecutarReaderStoredProcedure(string textoComando, CommandType tipoComando)
        {
            if (string.IsNullOrEmpty(textoComando.Trim()))
            {
                throw new ArgumentException("No se especifico el comando", "string textoComando");
            }

            IDataReader reader = null;
            IDbCommand comando = null;
            try
            {
                this.conexion.Open();

                comando = PrepararComando(textoComando, tipoComando, null);

                comando.CommandTimeout = 0;
                reader = comando.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                comando.Dispose();
                comando = null;
            }
            return reader;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado.
        /// </summary>
        /// <param name="commandText">Nombre del Procedimiento Almacenado.</param>
        /// <param name="parametros">Lista de Parámetros.</param>
        /// <returns>Objeto comando.</returns>
        public object EjecutarStoredProcedure(string textoComando, IDataParameter[] parametros)
        {
            if (string.IsNullOrEmpty(textoComando.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string textoComando");
            }

            object returnValue = null;
            IDbCommand comando = null;

            try
            {
                if (this.conexion.State == ConnectionState.Closed)
                {
                    this.conexion.Open();
                }
                comando = PrepararComando(textoComando, CommandType.StoredProcedure, parametros);
                comando.CommandTimeout = 0;
                returnValue = comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                comando.Dispose();
                comando = null;

                if (this.conexion.State == ConnectionState.Open)
                {
                    this.conexion.Close();
                }
            }

            return returnValue;
        }

        public object EjecutarStoredProcedureTransaccional(string textoComando, IDataParameter[] parametros, DbTransaction transaccion)
        {
            if (string.IsNullOrEmpty(textoComando.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string textoComando");
            }

            if (transaccion == null)
            {
                throw new ArgumentException("No se especificó la transaccion", "DbTransaction transaction");
            }

            object returnValue = null;
            IDbCommand comando = null;

            try
            {
                if (this.conexion.State == ConnectionState.Closed)
                {
                    this.conexion.Open();
                }
                comando = PrepararComando(textoComando, CommandType.StoredProcedure, parametros);
                comando.Transaction = transaccion;
                comando.CommandTimeout = 0;
                returnValue = comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                comando.Dispose();
            }

            return returnValue;
        }

        /// <summary>
        /// Ejecuta un Procedimiento almacenado y devuelve un data reader con la información recuperada.
        /// </summary>
        /// <param name="commandText">nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Lista de parámetros.</param>
        /// <returns>Objeto Data reader con la información recuperada.</returns>
        public IDataReader EjecutarReaderSP(string textoComando, IDataParameter[] parametros)
        {
            if (string.IsNullOrEmpty(textoComando.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string textoComando");
            }

            IDataReader reader = null;
            IDbCommand comando = null;
            try
            {
                if (this.conexion.State == ConnectionState.Closed)
                {
                    this.conexion.Open();
                }

                comando = PrepararComando(textoComando, CommandType.StoredProcedure, parametros);

                comando.CommandTimeout = 0;
                reader = comando.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                comando.Dispose();
                comando = null;
            }
            return reader;
        }

        /// <summary>
        /// Ejecuta un comando y regresa el valor dentro de un datatable
        /// </summary>
        /// <param name="textoComando">Texto del comando a ejecutar(storeprocedure, consulta)</param>
        /// <param name="parametros">lista de parametros que deben ejecutarse</param>
        /// <returns>Regresa un datatable con los datos obtenidos</returns>
        public DataTable EjecutarSentenciaTabla(string textoComando, IDataParameter[] parametros)
        {
            if (string.IsNullOrEmpty(textoComando.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string commandText");
            }

            DataSet dataSet = null;
            IDbDataAdapter sqlDataAdapter = null;
            IDbCommand comando = null;
            try
            {
                this.conexion.Open();
                dataSet = new DataSet();

                sqlDataAdapter = new SqlDataAdapter();
                comando = PrepararComando(textoComando, CommandType.StoredProcedure, parametros);
                sqlDataAdapter.SelectCommand = comando;
                sqlDataAdapter.Fill(dataSet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                comando.Dispose();
                comando = null;
                if (this.conexion.State == ConnectionState.Open)
                {
                    this.conexion.Close();
                }
            }

            DataTable tabla = null;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                tabla = dataSet.Tables[0].Copy();
            }
            dataSet.Dispose();

            return tabla;
        }

        /// <summary>
        /// Ejecuta una sentencia de SQL y devuelve el número de registros afectados.
        /// </summary>
        /// <param name="sql">Sentencia de SQL.</param>
        /// <param name="tipo">Tipo de comando a ejecutar.</param>
        /// <param name="parametros">Lista de parámetros.</param>
        /// <returns>Número de registros afectados.</returns>
        public int EjecutarSentencia(string sql, CommandType tipo, IDataParameter[] parametros)
        {
            if (string.IsNullOrEmpty(sql.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string sql");
            }

            int registrosAfectados = 0;
            IDbCommand comando = null;
            try
            {
                this.conexion.Open();
                comando = PrepararComando(sql, tipo, parametros);

                registrosAfectados = comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                comando.Dispose();

                if (this.conexion != null)
                {
                    this.conexion.Close();
                }
            }

            return registrosAfectados;
        }

        /// <summary>
        /// Ejecuta una sentencia de SQL dentro de una transacción.
        /// </summary>
        /// <param name="sql">Sentencia de SQL.</param>
        /// <param name="tipo">Tipo de comando a ejecutar.</param>
        /// <param name="parametros">Lista de parámetros.</param>
        /// <param name="transaccion">Objeto transacción.</param>
        /// <returns>Número de registros afectados.</returns>
        public int EjecutarSentenciaTransaccional(string sql, CommandType tipo, IDataParameter[] parametros, DbTransaction transaccion)
        {
            if (string.IsNullOrEmpty(sql.Trim()))
            {
                throw new ArgumentException("No se especificó el comando", "string sql");
            }

            if (transaccion == null)
            {
                throw new ArgumentException("No se especificó la transaccion", "DbTransaction transaction");
            }

            int recordsAffected = 0;
            IDbCommand command = null;
            try
            {
                command = PrepararComando(sql, tipo, parametros);
                command.Transaction = transaccion;

                recordsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();
            }
            return recordsAffected;
        }

        public void Dispose()
        {
            CerrarConexion();
        }
    }
}
