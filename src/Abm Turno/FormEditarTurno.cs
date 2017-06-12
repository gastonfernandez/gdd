using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using UberFrba.Mappings;

namespace UberFrba.Abm_Turno
{
    public partial class FormEditarTurno : Form
    {
        Validacion v = new Validacion();
        SqlConnection conexion;
        String TurnoId = null;
        List<FormEditarTurno> turnosAsociados = new List<FormEditarTurno>();

        public FormEditarTurno()
        {
            InitializeComponent();
            conexion = new SqlConnection(@Config.strConnection);
        }

        public void cargarDatos(String numeroTurno)
        {
            TurnoId = numeroTurno;

            conexion.Open();

            String query = "SELECT * FROM OSNR.Turno WHERE tur_id = '" + numeroTurno + "'";

            SqlCommand listar = new SqlCommand(query, conexion);

            DataTable tabla = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = listar;
            adapter.Fill(tabla);

            txtDescripcion.Text = tabla.Rows[0]["tur_descripcion"].ToString();
            txtInicio.Text = tabla.Rows[0]["tur_hora_inicio"].ToString();
            txtFin.Text = tabla.Rows[0]["tur_hora_fin"].ToString();
            txtValorKm.Text = tabla.Rows[0]["tur_valor_km"].ToString();
            txtBase.Text = tabla.Rows[0]["tur_precio_base"].ToString();
            
            conexion.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (camposCompletos())
            {
                if (seSuperponenHoras())
                {
                    MessageBox.Show("El horario del turno se superpone con uno de un turno ya existente");
                }
                if (masDe24Horas())
                {
                    MessageBox.Show("El horario del turno no puede durar mas de 24 horas");
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    guardarDatos();
                    MessageBox.Show("Datos guardados correctamente!");
                    this.Close();
                }
            }
        }

        private bool seSuperponenHoras()
        {
            //TODO
            return false;
        }

        private bool masDe24Horas()
        {
            int horaInicio = Convert.ToInt32(txtInicio.Text);
            int horaFin = Convert.ToInt32(txtFin.Text);
            int cantHoras = horaFin - horaInicio;

            if (cantHoras <= 24 && cantHoras > 0) 
            {
                return false;
            }
            return true;

        }

        private bool horaInvalida(String hora)
        {
            int horaNum = Convert.ToInt32(hora);
            if (horaNum >= 0 && horaNum <= 24)
            {
                return false;
            }
            return true;
        }
        
        private void guardarDatos()
        {
            Dictionary<String, DbTypedValue> campos = new Dictionary<String, DbTypedValue>();
            campos.Add("TurnoId", new DbTypedValue(TurnoId, SqlDbType.Decimal));
            campos.Add("Descripcion", new DbTypedValue(txtDescripcion.Text, SqlDbType.VarChar));
            campos.Add("Hora Inicio", new DbTypedValue(txtInicio.Text, SqlDbType.Decimal));
            campos.Add("Hora Fin", new DbTypedValue(txtFin.Text, SqlDbType.Decimal));
            campos.Add("Valor Km", new DbTypedValue(txtValorKm.Text, SqlDbType.Decimal));
            campos.Add("Precio Base", new DbTypedValue(txtBase.Text, SqlDbType.Decimal));
            
            Dictionary<int, String> errorMensaje = new Dictionary<int, string>();
            new BaseDeDatos().ExecSP("OSNR.ModificarOCrearTurno", campos, errorMensaje);
        }

        private bool camposCompletos()
        {
            if (txtDescripcion.Text == "")
            {
                MessageBox.Show("Complete la descripcion");
            }
            else if (txtInicio.Text == "")
            {
                MessageBox.Show("Complete la hora de inicio");
            }
            else if (horaInvalida(txtInicio.Text))
            {
                MessageBox.Show("La hora de inicio debe ser un valor entre 0 y 24");
            }
            else if (horaInvalida(txtFin.Text))
            {
                MessageBox.Show("La hora de fin debe ser un valor entre 0 y 24");
            }
            else if (txtFin.Text == "")
            {
                MessageBox.Show("Complete la hora de fin");
            }
            else if (!esNumerico(txtInicio.Text))
            {
                MessageBox.Show("La hora de inicio debe contener solo numeros");
            }
            else if (!esNumerico(txtFin.Text))
            {
                MessageBox.Show("La hora de fin debe contener solo numeros");
            }
            else if (!esNumerico(txtValorKm.Text))
            {
                MessageBox.Show("El valor del km debe contener solo numeros");
            }
            else if (txtValorKm.Text == "")
            {
                MessageBox.Show("Complete el valor del km");
            }
            else if (txtBase.Text == "")
            {
                MessageBox.Show("Complete el precio base");
            }
            else if (!esNumerico(txtBase.Text))
            {
                MessageBox.Show("El precio base debe contener solo numeros");
            }
            else
            {
                return true;
            }
            return false;
        }

        private bool esNumerico(String cadena)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(cadena, @"^\d+$");
        }

        private void txtInicio_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void txtFin_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void txtValorKm_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

        private void txtBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            v.soloNumeros(e);
        }

    }
}
