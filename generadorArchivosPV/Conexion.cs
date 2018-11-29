using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generadorArchivosPV
{
    class Conexion
    {
        private Conexion() { }

        public static SqlConnection ObtenerConexion()
        {
            return InternaHolder.GetInstance();
        }

        public static SqlCommand ObtenerComandoFinal(String sql, SqlConnection conn)
        {
            return InternaHolder.GetComando(sql, conn);
        }

        public static int ObtieneCantidadObjetos()
        {
            return InternaHolder.getNumeroObjetos();
        }

        public static void RetornaPool()
        {
            InternaHolder.Cerrar();
        }

        // clase interna Holder 
        private static class InternaHolder
        {
            private static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LINEAPRUEBASEntities"].ConnectionString);

            private static SqlCommand comando = new SqlCommand();

            private static int numeroObjetos = 1;

            public static int getNumeroObjetos()
            {
                return numeroObjetos;
            }

            public static SqlCommand GetComando(String sql, SqlConnection conn)
            {

                if (comando == null)
                {
                    comando = new SqlCommand(sql, conn);
                    numeroObjetos = numeroObjetos + 1;
                }
                else
                {
                    comando.CommandText = sql;
                    comando.Connection = conn;
                }


                return comando;
            }

            public static SqlConnection GetInstance()
            {
                if (connection == null)
                {
                    connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LINEAPRUEBASEntities"].ConnectionString);

                    connection.Open();


                }
                else
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                }


                return connection;
            }

            public static void Cerrar()
            {
                if (!(connection == null))
                {
                    connection.Close();
                }

            }

        }
    }
}
