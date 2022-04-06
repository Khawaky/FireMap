using FireSharp;
using System.Collections.Generic;
using FireSharp.Response;
using FireSharp.Config;
using FireSharp.Interfaces;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FireMap
{

    public partial class Form1 : Form
    {
        GMarkerGoogle marker;
        GMapOverlay makerOverlay;
        int a = 0;

        double LatInicial = 27.072374836035127;
        double LngInicial = -109.43840056242351;

        public Form1()
        {
            InitializeComponent();

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(LatInicial, LngInicial);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 22;
            gMapControl1.AutoScroll = true;

            //Agregar marcador
            makerOverlay = new GMapOverlay("Marcador");
            marker = new GMarkerGoogle(new PointLatLng(LatInicial, LngInicial), GMarkerGoogleType.green);
            makerOverlay.Markers.Add(marker);

            //Descripcion al marcador 
            marker.ToolTipMode = MarkerTooltipMode.Always;
            marker.ToolTipText = String.Format("Ubicación: Casa de Axel \n Latitud: {0} \n Longitud: {1}", LatInicial, LngInicial);

            //Se agrega lo anterior al mapa
            gMapControl1.Overlays.Add(makerOverlay);

        }

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "fNXWCS80uNPoEYhB4xEgQIxVTp40yGn5qtcaiaPx",
            BasePath = "https://fir-conexion-1fcd1-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FirebaseClient(ifc);
                MessageBox.Show("Conexión Exitosa");
            }
            catch
            {

                MessageBox.Show("Hay un problema con tu conexión a internet");
            }

        }

        private async void btnAgregar_Click(object sender, EventArgs e)
        {

           
            Coordenadas coo = new Coordenadas()
            {
                Ubi = txtUbi.Text,
                Lat = Convert.ToDouble(textBox2.Text),
                Lng = Convert.ToDouble(textBox3.Text) 
            };

            a++;

            var setter = client.Set("Ubicaciones/"+a,coo);
            MessageBox.Show("Se ha agregado la ubicacion con exito");

            gMapControl1.Position = new PointLatLng(coo.Lat, coo.Lng);

            //Agregar marcador
            makerOverlay = new GMapOverlay("Marcador");
            marker = new GMarkerGoogle(new PointLatLng(coo.Lat, coo.Lng), GMarkerGoogleType.red);
            makerOverlay.Markers.Add(marker);

            //Descripcion al marcador 
            marker.ToolTipMode = MarkerTooltipMode.Always;
            marker.ToolTipText = String.Format("Ubicación: {0} \n Latitud: {1} \n Longitud: {2}", coo.Ubi,coo.Lat, coo.Lng);

            //Se agrega lo anterior al mapa
            gMapControl1.Overlays.Add(makerOverlay);
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Visualizar_Click(object sender, EventArgs e)
        {
            FirebaseResponse res = client.Get(@"Ubicaciones");
            Dictionary<Object, Coordenadas> data = JsonConvert.DeserializeObject<Dictionary<Object, Coordenadas>>(res.Body.ToString());
            PopulateRTB(data);
        }

        void PopulateRTB(Dictionary < Object, Coordenadas > record)
        {
            richTextBox1.Clear();

            foreach (var item in record)
            {
                richTextBox1.Text += item.Key + "\n";
                richTextBox1.Text += "Ubicación" + item.Value.Ubi + "\n";
                richTextBox1.Text += "Latitud" + item.Value.Lat + "\n";
                richTextBox1.Text += "Longitud" + item.Value.Lng + "\n";

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
