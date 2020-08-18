using ExamenWebSalinas.AccesoDatos;
using ExamenWebSalinas.Entidades.ControlUsuarios;
using ExamenWebSalinas.Entidades.InicioSesion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vitamedica.Herramientas.AccesoDatos.Entidades;

namespace ExamenWebSalinas.Negocio.InicioSesion
{
    public class ProcesaDatosUsuario
    {

        public SesionLogin ObtenerUsuario(string usuario, string contresana)
        {
            try
            {
                SesionLogin login = null;
                var acceso = new AdministraAccesoDatos(Configuracion.BASEDATOSLOCAL, TipoProveedor.SqlServer);
                List<IDataParameter> listaParametros = new List<IDataParameter>();
                listaParametros.Add(new SqlParameter("@USUARIO", usuario));
                listaParametros.Add(new SqlParameter("@CONTRASENA", contresana));
                using (IDataReader reader = acceso.EjecutarReaderSP("[dbo].[OBTIENE_USUARIO]", listaParametros.ToArray<IDataParameter>()))
                {
                    while (reader.Read())

                    {
                        login = new SesionLogin();
                        {
                            login.UsuarioLogin = reader["Nombre"] == DBNull.Value ? "" : reader["Nombre"].ToString();
                            login.Nombre = reader["Apellidos"] == DBNull.Value ? "" : reader["Apellidos"].ToString();
                            login.Apellidos = reader["Usuario"] == DBNull.Value ? "" : reader["Usuario"].ToString();
                        };
                    }
                }
                return login;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<Usuarios> ObtenerUsuarios()
        {
            AdministraAccesoDatos acceso = null;
            List<IDataParameter> listaParametros = null;
            List<Usuarios> resultados = new List<Usuarios>();
            try
            {
                acceso = new AdministraAccesoDatos(Configuracion.BASEDATOSLOCAL, TipoProveedor.SqlServer);
                listaParametros = new List<IDataParameter>();
                using (DataTable TablaUsuarios = acceso.EjecutarSentenciaTabla("[dbo].[OBTIENER_USUARIOS]", listaParametros.ToArray<IDataParameter>()))
                {
                    if (TablaUsuarios.Rows.Count > 0)
                    {
                        foreach (DataRow Renglon in TablaUsuarios.Rows)
                        {
                            Usuarios usuarios = new Usuarios()
                            {
                                IdUsuario = Convert.ToInt32(Renglon["IdUsuario"].ToString()),
                                Nombre = Renglon["Nombre"] == DBNull.Value ? "" : Renglon["Nombre"].ToString(),
                                Apellidos = Renglon["Apellidos"] == DBNull.Value ? "" : Renglon["Apellidos"].ToString(),
                                Usuario = Renglon["Usuario"] == DBNull.Value ? "" : Renglon["Usuario"].ToString(),
                                Contrasena = Renglon["Contrasena"] == DBNull.Value ? "" : Renglon["Contrasena"].ToString(),

                            };
                            resultados.Add(usuarios);
                        }
                    }
                }
                return resultados;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (acceso != null)
                {
                    acceso.CerrarConexion();
                }
            }
        }



        public bool EliminarUsuario(int IdUsuario)
        {

            System.Data.Common.DbTransaction transaccion = null;
            AdministraAccesoDatos acceso = null;
            List<IDataParameter> listaParametros = null;

            try
            {
                acceso = new AdministraAccesoDatos(Configuracion.BASEDATOSLOCAL, TipoProveedor.SqlServer);

                acceso.ObtenerConexion().Open();
                transaccion = acceso.ObtenerConexion().BeginTransaction();

                listaParametros = new List<IDataParameter>();

                listaParametros.Add(new SqlParameter("@IDUSUARIO", IdUsuario));

                acceso.EjecutarStoredProcedureTransaccional("[dbo].[ELIMINA_USUARIO]", listaParametros.ToArray<IDataParameter>(), transaccion);

                transaccion.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaccion != null)
                {
                    transaccion.Rollback();
                    transaccion.Dispose();
                }

                throw ex;
            }
            finally
            {
                if (acceso != null)
                {
                    acceso.CerrarConexion();
                }

                if (transaccion != null)
                {
                    transaccion.Dispose();
                }
            }
        }



        public bool ActualizarUsuario(Usuarios model)
        {

            System.Data.Common.DbTransaction transaccion = null;
            AdministraAccesoDatos acceso = null;
            List<IDataParameter> listaParametros = null;

            try
            {
                acceso = new AdministraAccesoDatos(Configuracion.BASEDATOSLOCAL, TipoProveedor.SqlServer);

                acceso.ObtenerConexion().Open();
                transaccion = acceso.ObtenerConexion().BeginTransaction();

                listaParametros = new List<IDataParameter>();

                listaParametros.Add(new SqlParameter("@IDUSUARIO", model.IdUsuario));
                listaParametros.Add(new SqlParameter("@CONTRASENA", model.Contrasena));

                acceso.EjecutarStoredProcedureTransaccional("[dbo].[ACTUALIZAR_USUARIO]", listaParametros.ToArray<IDataParameter>(), transaccion);

                transaccion.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaccion != null)
                {
                    transaccion.Rollback();
                    transaccion.Dispose();
                }

                throw ex;
            }
            finally
            {
                if (acceso != null)
                {
                    acceso.CerrarConexion();
                }

                if (transaccion != null)
                {
                    transaccion.Dispose();
                }
            }
        }


        public bool RegistrarUsuario(Usuarios model)
        {

            System.Data.Common.DbTransaction transaccion = null;
            AdministraAccesoDatos acceso = null;
            List<IDataParameter> listaParametros = null;

            try
            {
                acceso = new AdministraAccesoDatos(Configuracion.BASEDATOSLOCAL, TipoProveedor.SqlServer);

                acceso.ObtenerConexion().Open();
                transaccion = acceso.ObtenerConexion().BeginTransaction();

                listaParametros = new List<IDataParameter>();

                listaParametros.Add(new SqlParameter("@NOMBRE", model.Nombre));
                listaParametros.Add(new SqlParameter("@APELLIDO", model.Apellidos));
                listaParametros.Add(new SqlParameter("@USUARIO", model.Usuario));
                listaParametros.Add(new SqlParameter("@CONTRASENA", model.Contrasena));

                acceso.EjecutarStoredProcedureTransaccional("[dbo].[REGISTRA_USUARIO]", listaParametros.ToArray<IDataParameter>(), transaccion);

                transaccion.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaccion != null)
                {
                    transaccion.Rollback();
                    transaccion.Dispose();
                }

                throw ex;
            }
            finally
            {
                if (acceso != null)
                {
                    acceso.CerrarConexion();
                }

                if (transaccion != null)
                {
                    transaccion.Dispose();
                }
            }
        }

    }
}
