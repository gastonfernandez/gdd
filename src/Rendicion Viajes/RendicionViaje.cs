using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Mappings;

namespace UberFrba.Rendicion_Viajes
{
    /*
     * Funcionalidad que permite a la empresa realizar el pago, de los viajes que realizó
        el chofer, en un turno determinado. El chofer recibirá el pago de los viajes realizados en
        el día (y en su turno correspondiente), dicho pago se establece como un porcentaje de
        cada uno de los viajes realizados por el mismo.
        Para hacer frente a esta funcionalidad es necesario que se registre los siguientes
        campos:
         Fecha.
         Chofer
         Turno
         Importe total de la rendición.
        Todos los datos mencionados anteriormente son obligatorios.
        Hay que tener en cuenta que no se puede rendir 2 veces el mismo viaje, no hay
        más de una rendición por día. El chofer cobra en el día el monto de todos los viajes que
        realizó en su jornada.
        Todo chofer al cual se le haga una rendición debe ser un chofer activo/habilitado.
        Será necesario que al momento de calcularse la rendición de cuenta se muestre una
        tabla que informe en forma detallada los viajes que componen la rendición y que
        justifiquen el importen total generado por ese chofer.
     * */

    public partial class RendicionViaje : Form
    {
        private int porcentaje = Config.porcentajeRendicion;

        public RendicionViaje()
        {
            InitializeComponent();
        }

        private void RendicionViaje_Load(object sender, EventArgs e)
        {
            this.comboTurnos.DisplayMember = "tur_descripcion";

            this.comboTurnos.DataSource = new BaseDeDatos().select_query("SELECT * FROM OSNR.Turno");
        }

    }
}
