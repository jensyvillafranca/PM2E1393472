using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Maps;
namespace PM2E1393472.Vistas;

public partial class PageMap : ContentPage
{
    ModeloSQL.Sitios sitios;
    public PageMap()
	{
		InitializeComponent();
	}
    public PageMap(ModeloSQL.Sitios itemSeleccionado)
    {
        InitializeComponent();
        sitios = new ModeloSQL.Sitios();
        sitios = itemSeleccionado;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var connection = Connectivity.NetworkAccess;
        var local = Geolocation.GetLocationAsync();

        if (connection == NetworkAccess.Internet)
        {
            var location = await local;
            if (location != null)
            {
                var pinEstatico = new Pin
                {
                    Type = PinType.Place,
                    Location = new Location(Convert.ToDouble(sitios.latitud), Convert.ToDouble(sitios.longitud)),
                    Label = sitios.descripcion,
                };

                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Convert.ToDouble(sitios.latitud), Convert.ToDouble(sitios.longitud)), Distance.FromKilometers(1)));
                mapa.Pins.Add(pinEstatico);
                mapa.IsShowingUser = true; //Aqui es donde utilizo el isShowingUser.
            }
            else
            {
                if (!local.IsCompletedSuccessfully)
                {
                    await DisplayAlert("GPS desactivado", "Por favor, activa el GPS para continuar.", "OK");
                }
            }
        }
        else
        {
            await DisplayAlert("Sin Acceso a internet", "Por favor, revisa tu conexión a internet para continuar.", "OK");
        }
    }

    private async void LoadImage(object sender, EventArgs e)
    {
      //  await ShareImage(sitios.Imagen, "ubicacion.jpg");
    }

    private async Task ShareImage(byte[] imageData, string filename)
    {
        var file = Path.Combine(FileSystem.CacheDirectory, filename);
        File.WriteAllBytes(file, imageData);

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Compartir Imagen de Sitio",
            File = new ShareFile(file)
        });
    }

}