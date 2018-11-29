using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace generadorArchivosPV
{
    class Program
    {
        static void Main(string[] args)
        {
            

            Generador g = new Generador();
            var salir = string.Empty;
            do
            {
                g.Generar();
                /*Console.WriteLine("Presione S para salir");
                salir = Console.ReadLine().ToLower();*/
            } while (salir != "S");
        }
    }

    class Generador
    {
        

        string Ruta = "C:\\temp\\ArchivosGenerados";
        string Serie = "X";
        string Fecha = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}"; 
        string Nit = "6789362-7";
        string Nombre = "Archivo1";
        int NoDocumento = 1;
        int CantidadDocumentos = 1;
        int NombreCount = 1;
        string mensajeError;

        public void Generar()
        {
            try
            {
                Console.WriteLine("Quiere generar el siguiente documento?, S = Si");
                var GenerarSiguiente = Console.ReadLine();

                if (GenerarSiguiente.ToUpper() != "S")
                {
                    //recepción de datos
                    mensajeError = "Error en la toma de datos ";
                    TomaDeDatos();
                }


                Console.WriteLine("Cuantos documentos quieres generar?");
                int numeroAGenerar = 1;
                mensajeError = "Error, se tiene que reciir un valor numerico en la toma del numero documentos a generar";
                numeroAGenerar = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < numeroAGenerar; i++)
                {
                    var NoDocumentoFinal = (CantidadDocumentos + NoDocumento);

                    //Proceso de generacion de Documento
                    mensajeError = "Error al generar el texto del archivo ";
                    var DocumentoFinal = CrearDocumento(NoDocumentoFinal);

                    //Proceso de generacion de archivo
                    mensajeError = "Error en la Generación del archivo ";
                    DescargaDeArchivo(DocumentoFinal, NoDocumentoFinal);
                }
            }
            catch (Exception es)
            {
                Console.Clear();
                Console.WriteLine($"{mensajeError}:{ es.Message}");
            }
        }

        public void TomaDeDatos()
        {
            //recepción de datos
            Console.ForegroundColor = ConsoleColor.White;
            string v;
            Console.WriteLine("Ingrese el nombre que se le asignara al archivo");
            v = Console.ReadLine();

            if (v != "")
            {
                NombreCount = 1;
            }

            Nombre = (v == "") ? Nombre : v;
            Console.WriteLine("Ingrese la ruta a la cual se descargara el documento");
            v = Console.ReadLine();
            Ruta = (v == "") ? Ruta : v;
            Console.WriteLine("Ingrese la serie de los documentos");
            v = Console.ReadLine();
            Serie = (v == "") ? Serie : v;
            Console.WriteLine("ingrese la fecha del documento");
            v = Console.ReadLine();
            Fecha = (v == "") ? Fecha : v;
            Console.WriteLine("ingrese el nit del documento");
            v = Console.ReadLine();
            Nit = (v == "") ? Nit : v;
            
            // Buscar el IdEmpresa por medio del NIT
            int numBuscado = BusquedaUltimoDocumento();
            
            Console.WriteLine("ingrese el numero de documento proximo a registrar");
            v = Console.ReadLine();
            NoDocumento = (v == "") ? numBuscado : Int32.Parse(v);
            Console.WriteLine("ingrese la cantidad de documentos a crear");
            v = Console.ReadLine();
            CantidadDocumentos = (v == "") ? CantidadDocumentos : Int32.Parse(v);
        }

        public string CrearDocumento(int NoDocumentoFinal)
        {
            //Proceso de generacion de archivo
            var DocumentoFinal = string.Empty;

            /*switch (TipoDocumento)
            {
                case "1":
                case "2":
                case "3":
                default:
            }*/
            
            for (int i = NoDocumento; i < NoDocumentoFinal; i++)
            {
                var documento = $"E01|FACE|{Serie}|{i}|1||{Fecha} 00:00:00|ORIGINAL||||{Nit}|01|32233-4|Cliente de Prueba GT||Guatemala Capital|||||||||GTQ|1.00|1||0.00|0.00||12.00|0.00|112.00|Ciento Doce con 00/100|FACE|0123456789\r\n"
                + $"D01||1234567890abcdefghoq|producto descripcion 1|1.000|B|UNI|112.00|0.00|0.00|112.00|0.00|100.00|12.00\r\n"
                + $"D02|IVA|100.00|12.00|12\r\n"
                + $"I01|IVA|100.00|12.00|12\r\n";

                DocumentoFinal = DocumentoFinal + documento;
            }
            var pieDocumento = $"T01|{NoDocumento}|{(NoDocumentoFinal - 1)}|{CantidadDocumentos}|{Nombre}-{NombreCount}.txt";
            DocumentoFinal = DocumentoFinal + pieDocumento;

            return DocumentoFinal;
        }

        public void DescargaDeArchivo(string DocumentoFinal, int NoDocumentoFinal)
        {
            var rutaFinal = Path.Combine(Ruta, $"{Nombre}-{NombreCount}.txt");
            File.WriteAllText(rutaFinal, DocumentoFinal, Encoding.UTF8);
            Console.Clear();
            Console.WriteLine("Documento siguiente: " + NoDocumentoFinal);
            NoDocumento = NoDocumentoFinal;
            NombreCount++;
        }

        public int BusquedaUltimoDocumento()
        {
            int num = 0;
            try
            {
                int id = 0;
                string queryEmp = $"Select TOP 1 IdEmpresa from SSO_SEG_EMPRESA Where NIT = '{Nit}'";
                var command = Conexion.ObtenerComandoFinal(queryEmp, Conexion.ObtenerConexion());
                SqlDataReader idEmpresa = command.ExecuteReader();
                while (idEmpresa.Read())
                {
                    id = Int32.Parse(idEmpresa["IdEmpresa"].ToString());
                }
                idEmpresa.Close();

                string queryDocs = $"Select TOP 1 NoDocumento from SSO_IFL_DOCUMENTO Where IdEmpresa = {id.ToString()} and Serie = '{Serie}' Order by NoDocumento Desc ";
                command = Conexion.ObtenerComandoFinal(queryDocs, Conexion.ObtenerConexion());
                SqlDataReader areas = command.ExecuteReader();

                while (areas.Read())
                {
                    num = Int32.Parse(areas["NoDocumento"].ToString()) + 1;
                }
                areas.Close();
            }
            catch (Exception es)
            {
                Console.WriteLine("Error: " + es.Message);
            }

            return num;
        }
    }
}
