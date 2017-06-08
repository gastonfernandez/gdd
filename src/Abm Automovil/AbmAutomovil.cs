using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba;
using UberFrba.Mappings;

namespace autom
{
    public partial class AbmAutomovil : Form
    {

        public AbmAutomovil()
        {
            InitializeComponent();
            BuscarVehiculos();
            LlenarComboMarca();
        }

        public void LlenarComboMarca()
        {
            List<Combo> lista = new List<Combo>();
            //BaseDeDatos db = new BaseDeDatos();
            //DataTable dt = db.select_query("select distinct mar_id, mar_nombre from marca");//todas las marcas posibles
            
            //foreach (DataRow row in dt.Rows)
            //    lista.Add(new Combo(Convert.ToString(row["mar_nombre"]), Convert.ToInt32(row["mar_id"])));
            //comboMarca.DisplayMember = "mar_id";
            lista.Add(new Combo("Chevrolet",1));//sacar
            comboMarca.ValueMember = "mar_nombre";
            comboMarca.DataSource = lista; 
        }

        public void BuscarVehiculos()
        {
            listaConsulta.Items.Clear();
            //BaseDeDatos db = new BaseDeDatos();

            //String query = "select distinct v.veh_id,v.veh_patente,mo.mod_nombre, ma.mar_nombre, u.usu_nombre,v.veh_habilitado ";
            //query += "from vehiculo v ";
            //query += "join chofer ch on ch.cho_id = v.veh_id_chofer ";
            //query += "join usuario u on u.usu_id = ch.cho_id_usuario ";
            //query += "join modelo mo on mo.mod_id = v.veh_modelo_id ";
            //query += "join marca ma on ma.mar_id = mo.mod_marca_id ";
            //query += "where ";
            //query += "mo.mod_nombre like '%" + txtModelo.Text + "%' ";
            //query += "and mo.mar_nombre like '%" + comboMarca.Text + "%' "; //revisar
            //query += "and v.veh_patente like '%" + txtPatente.Text + "%' ";
            //query += "and u.usu_nombre like '%" + txtChofer.Text + "%' ";
            //query += "order by veh_habilitado desc, veh_patente ";

            //DataTable dt = db.select_query(query);

            //foreach (DataRow row in dt.Rows)
            //{
            //    ListViewItem item = new ListViewItem(Convert.ToString(row["veh_id"]));//no deberia mostrarse
            //    item.SubItems.Add(Convert.ToString(row["veh_patente"]));
            //    item.SubItems.Add(Convert.ToString(row["mod_nombre"]));
            //    item.SubItems.Add(Convert.ToString(row["mar_nombre"]));
            //    item.SubItems.Add(Convert.ToString(row["usu_nombre"]));
            //    item.SubItems.Add(Convert.ToByte(row["veh_habilitado"]) == 1 ? "Activo" : "Inactivo");

            //    listaConsulta.Items.Add(item);
            //}

            ListViewItem itme = new ListViewItem("1111111");
            itme.SubItems.Add("adfgadg");
            itme.SubItems.Add("adfgadg");
            itme.SubItems.Add("gadfgasdf");
            itme.SubItems.Add("gasdfgas");
            itme.SubItems.Add("Inactivo");
            listaConsulta.Items.Add(itme);

            ListViewItem assa = new ListViewItem("111");
            assa.SubItems.Add("adfvzxcvxcgadg");
            assa.SubItems.Add("adfvzdcxvzxgadg");
            assa.SubItems.Add("gadvzxcvfgasdf");
            assa.SubItems.Add("gazxcvsdfgas");
            assa.SubItems.Add("Activo");
            listaConsulta.Items.Add(assa);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarVehiculos();
        }

        private void altaVehiculo_Click(object sender, EventArgs e)
        {
            ListViewItem itm = null;
            AltaModVehiculo amv = new AltaModVehiculo(itm, this);
            amv.Show();
            this.Hide();   
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (listaConsulta.SelectedItems.Count == 0)
                MessageBox.Show("Debe seleccionar un item de la lista");

            foreach (ListViewItem item in listaConsulta.SelectedItems)
            {
                if(item.SubItems[5].Text == "Activo")
                    eliminarVehiculo(Convert.ToInt64(item.Text));
                else if (item.SubItems[5].Text == "Inactivo")
                    MessageBox.Show("El Automovil ya se encuentra INACTIVO");
            }
        }

        public void eliminarVehiculo(Int64 id) 
        {
            try
            {
                BaseDeDatos db = new BaseDeDatos();
                db.query("update vehiculo set veh_habilitado = 0 where veh_id = " + id);
            }
            catch
            {
                MessageBox.Show("El Automovil no pudo eliminarse correctamente. Intentelo nuevamente.");
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

        }
    }

}
