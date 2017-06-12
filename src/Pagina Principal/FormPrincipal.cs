using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Abm_Turno;
using UberFrba.Abm_Rol;
using UberFrba.Mappings;
using UberFrba.Pagina_Principal;
//using UberFrba.Registro_Viajes;
using UberFrba.Rendicion_Viajes;
using UberFrba.Facturacion;
using UberFrba.Listado_Estadistico;
using autom;
using UberFrba.Abm_Cliente;
using UberFrba.Abm_Chofer;
using UberFrba.Registro_Viajes;

namespace UberFrba
{
    public partial class FormPrincipal : Form
    {
        public Dictionary<int, ToolStripMenuItem> ids_funcionalidades = new Dictionary<int, ToolStripMenuItem>();
        BaseDeDatos db = new BaseDeDatos();
        Usuario usuarioLogueado;

        public FormPrincipal()
        {  
            InitializeComponent();
            ids_funcionalidades.Add(1, aBMRolToolStripMenuItem);
            ids_funcionalidades.Add(2, aBMClienteToolStripMenuItem);
            ids_funcionalidades.Add(3, aBMChoferToolStripMenuItem);
            ids_funcionalidades.Add(4, aBMAutomovilToolStripMenuItem);
            ids_funcionalidades.Add(5, aBMTurnoToolStripMenuItem);
            ids_funcionalidades.Add(6, registroDeViajeToolStripMenuItem);
            ids_funcionalidades.Add(7, rendicionDeViajeToolStripMenuItem);
            ids_funcionalidades.Add(8, facturacionDeClientesToolStripMenuItem);
            ids_funcionalidades.Add(9, listadoEstadisticoToolStripMenuItem);
        }

        internal void Show(Mappings.Usuario usu)
        {
            this.usuarioLogueado = usu;
            this.Show();
        }

        public void resetearFuncionalidades() 
        {
            resetMenuItems(menuStrip1.Items);
            loginToolStripMenuItem.Visible = true;
        }

        private void resetMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripMenuItem item in items)
            {
                if (item.HasDropDownItems)
                {
                    item.Visible = true;
                    resetMenuItems(item.DropDownItems);
                }
                else
                {
                    item.Visible = false;
                }
            }
        }

        public void activarFuncionalidad(int idFuncion) 
        {
            ids_funcionalidades[idFuncion].Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            resetearFuncionalidades();

            foreach (Rol rol in usuarioLogueado.roles)
            {
                if (rol.seleccionado)
                {
                    foreach (Int32 idFuncionalidad in rol.ObtenerFuncionalidades()) 
                    {
                        activarFuncionalidad(idFuncionalidad);
                    }
                }
            }
        }

        private void abrirFormulario(Form form)
        {
            form.MdiParent = this;
            form.Show();
        }

        private void aBMRolToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            abrirFormulario(new AbmRol());
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new InicioSesion().Show();
            this.Close();
        }

        private void aBMClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new AbmCliente(false));
        }

        private void aBMChoferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new AbmChofer(false));
        }

        private void aBMAutomovilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new AbmAutomovil());
        }

        private void aBMTurnoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new AbmTurno());
        }

        private void registroDeViajeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new RegistroViaje());
        }

        private void rendicionDeViajeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new RendicionViaje());
        }

        private void facturacionDeClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new FacturacionCliente());
        }

        private void listadoEstadisticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abrirFormulario(new ListadoEstadistico());
        }

    }

}
