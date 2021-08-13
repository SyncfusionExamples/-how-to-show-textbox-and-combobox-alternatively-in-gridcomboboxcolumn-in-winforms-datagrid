using Syncfusion.Data;
using Syncfusion.WinForms.DataGrid;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid.Events;
using Syncfusion.WinForms.Input.Enums;
using Syncfusion.WinForms.DataGrid.Enums;
using Syncfusion.WinForms.ListView;
using Syncfusion.WinForms.ListView.Enums;
using Syncfusion.WinForms.DataGrid.Renderers;
using Syncfusion.WinForms.GridCommon.ScrollAxis;

namespace SfDataGridDemo
{
    public partial class Form1 : Form
    {
        #region Fields
        OrderInfoCollection orderInfo;

        static int shipCityID;
        #endregion

        #region Constuctor
        public Form1()
        {
            InitializeComponent();
            this.sfDataGrid1.AddNewRowPosition = RowPosition.Top;
            orderInfo = new OrderInfoCollection();
            this.sfDataGrid1.DataSource = orderInfo.OrdersListDetails;
            shipCityID = orderInfo.ShipCityDetails.Count;
            this.sfDataGrid1.Columns.Add(new GridNumericColumn() { MappingName = "OrderID", HeaderText = "Order ID" });
            this.sfDataGrid1.Columns.Add(new GridTextColumn() { MappingName = "CustomerID", HeaderText = "Customer ID" });
            this.sfDataGrid1.Columns.Add(new GridTextColumn() { MappingName = "ContactNumber", HeaderText = "Contact Number" });
            this.sfDataGrid1.Columns.Add(new GridNumericColumn() { HeaderText = "Quantity", MappingName = "Quantity", FormatMode = FormatMode.Numeric });

            this.sfDataGrid1.Columns.Add(new GridNumericColumn() { MappingName = "Discount", HeaderText = "Discount", FormatMode = FormatMode.Percent });
            this.sfDataGrid1.Columns.Add(new GridDateTimeColumn() { MappingName = "OrderDate", HeaderText = "Order Date", FilterMode = ColumnFilter.DisplayText });
            this.sfDataGrid1.Columns.Add(new GridComboBoxColumn() { MappingName = "ShipCityID", HeaderText = "Ship City", DisplayMember = "ShipCityName", ValueMember = "ShipCityID", DropDownStyle = DropDownStyle.DropDown, DataSource = orderInfo.ShipCityDetails });

            this.sfDataGrid1.CellComboBoxSelectionChanged += sfDataGrid1_CellComboBoxSelectionChanged;
            this.sfDataGrid1.RowValidating += sfDataGrid1_RowValidating;

            this.sfDataGrid1.CurrentCellValidating += sfDataGrid1_CurrentCellValidating;
            //Remove existing ComboBox Renderer
            this.sfDataGrid1.CellRenderers.Remove("ComboBox");
            //Add customized ComboBox Renderer
            this.sfDataGrid1.CellRenderers.Add("ComboBox", new GridComboBoxCellRendererExt(sfDataGrid1));
        }

        void sfDataGrid1_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
        {
            if (this.sfDataGrid1.CurrentCell.Column is GridComboBoxColumn && this.sfDataGrid1.CurrentCell.CellRenderer.CurrentCellRendererElement != null)
            {
                var shipCityName = this.sfDataGrid1.CurrentCell.CellRenderer.CurrentCellRendererElement.Text;
                var shipCityDetails = orderInfo.ShipCityDetails.FirstOrDefault(city => city.ShipCityName == shipCityName);

                if (shipCityDetails == null && !string.IsNullOrEmpty(this.sfDataGrid1.CurrentCell.CellRenderer.CurrentCellRendererElement.Text))
                {
                    shipCityID++;
                    orderInfo.ShipCityDetails.Add(new ShipCityDetails() { ShipCityName = this.sfDataGrid1.CurrentCell.CellRenderer.CurrentCellRendererElement.Text, ShipCityID = shipCityID });
                }
            }
        }

        void sfDataGrid1_RowValidating(object sender, RowValidatingEventArgs e)
        {
            if (e.DataRow.RowIndex == this.sfDataGrid1.GetAddNewRowIndex())
            {
                if (this.sfDataGrid1.CurrentCell.Column is GridComboBoxColumn)
                    e.IsValid = false;
            }
        }

        void sfDataGrid1_CellComboBoxSelectionChanged(object sender, CellComboBoxSelectionChangedEventArgs e)
        {
            if (e.SelectedIndex == 1)
            {
                //Do your customizations here.
            }
        }

        #endregion
    }

    public class GridComboBoxCellRendererExt : GridComboBoxCellRenderer
    {
        private SfDataGrid dataGrid;

        public GridComboBoxCellRendererExt(SfDataGrid dataGrid) : base()
        {
            this.dataGrid = dataGrid;
        }

        protected override void OnInitializeEditElement(DataColumnBase column, RowColumnIndex rowColumnIndex, SfComboBox uiElement)
        {
            base.OnInitializeEditElement(column, rowColumnIndex, uiElement);

            if (column.GridColumn.MappingName == "ShipCityID")
            {
                //ShipCity Column display the TextBox cell and ComboBox alternative rows in SfDataGrid

                //Display the text box to edit the cell value instead of the combo box condition based. 
                if (rowColumnIndex.RowIndex % 2 == 0)
                {
                    //To display the edit element as text box. 
                    uiElement.DropDownStyle = DropDownStyle.DropDown;
                    uiElement.DropDownButton.Visible = false;
                }
            }

        }
    }
}
