using System.Collections.ObjectModel;

namespace PM2E1393472.Vistas;

public partial class SelectSitios : ContentPage
{
    private Controles.SitiosControl rutaTarea;
    public ObservableCollection<ModeloSQL.Sitios> Items { get; set; }
    ModeloSQL.Sitios itemSeleccionado = null;
    public SelectSitios(IEnumerable<ModeloSQL.Sitios> ItemPersonas)
    {
        InitializeComponent();
        BindingContext = new ModeloSQL.ModeloSelect(ItemPersonas);
        rutaTarea = new Controles.SitiosControl();
        Items = new ObservableCollection<ModeloSQL.Sitios>();

        var viewModel = new ModeloSQL.ModeloSelect(ItemPersonas);
        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        load_new_data();
    }

    private async void load_new_data()
    {
        var listaSitios = await rutaTarea.GetListSitios(); //Lista actual de datos 
        Items.Clear(); //Limpia la lista

        //Recorrerla y llenarla con la nueva info actualizada

        foreach (var persona in listaSitios) //cada registro de la tabla pasa a la variable persona
        {
            Items.Add(persona); //se agrega esos registros actualizados de nuevo a la lista.
        }
        this.BindingContext = this;
    }

    private async void btnAtras_Clicked(object sender, EventArgs e)
    {
        var page = new Vistas.PaginaInicial();
        await Navigation.PushAsync(page);
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        if (validate_item()) return;

        bool answer = await DisplayAlert("Confirmaci�n de Eliminaci�n", "�Est�s seguro de que quieres eliminar este registro?", "S�", "No");
        if (answer == true)
        {
            //Mandar a llamar el metodo de eliminar de PersonasControles
            await rutaTarea.DeleteSitio(itemSeleccionado);
            itemSeleccionado = null;
            load_new_data();
        }
    }

    private async void btnVerMapa_Clicked(object sender, EventArgs e)
    {
        if (validate_item()) return;

        var mapa = new Vistas.PageMap(itemSeleccionado);
        await Navigation.PushAsync(mapa);
    }

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        if (validate_item()) return;

        var actualizarPantalla = new Vistas.UpdateSitios();
        actualizarPantalla.BindingContext = itemSeleccionado;
        await Navigation.PushAsync(actualizarPantalla);
    }

    private bool validate_item()
    {
        if (itemSeleccionado == null)
        {
            DisplayAlert("Advertencia", "Selecciona una direccion de la lista", "OK");
            return true;
        }

        return false;
    }

    private async void OnTapped(object sender, SelectionChangedEventArgs e)
    {
        //Obtener el item que se ha seleccionado o tocado
        itemSeleccionado = e.CurrentSelection.FirstOrDefault() as ModeloSQL.Sitios;
    }
}