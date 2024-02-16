using GeolocatorPlugin;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace PM2E1393472.Vistas;

public partial class PaginaInicial : ContentPage
{
    FileResult photo; //Objeto Global
    private Controles.SitiosControl sitiosBD; //Sirve para inicializar la BD
    public PaginaInicial(Controles.SitiosControl dbPath)
	{
		InitializeComponent();
        sitiosBD = dbPath;
    }
    public PaginaInicial()
    {
        InitializeComponent();
        sitiosBD = new Controles.SitiosControl();

    }

    //Obtener la latitud y longitud de acuerdo a mi dirección gps
    public async Task GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                txtLatitud.Text = "" + location.Latitude;
                txtLongitud.Text = "" + location.Longitude;
            }

        }
        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Advertencia", fnsEx + "", "OK");
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Advertencia", "Esta aplicacion no puede funcionar si no tiene los permisos", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Advertencia", "GPS Desactivado", "OK");
        }
    }

    //Solicitar permisos de geolocalización
    private async Task CheckAndRequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            GetLocationAsync();
        }
        else if (status == PermissionStatus.Denied)
        {
            await DisplayAlert("Advertencia", "Esta aplicacion no puede funcionar si no tiene los permisos", "OK");
        }
    }
    public async Task CheckAndRequestPermissionAsync()
    {
        // Verificar si el permiso ya ha sido otorgado
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            // Solicitar el permiso
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        // Manejar el resultado del permiso
        if (status == PermissionStatus.Granted)
        {
            await DisplayAlert("Aviso", "Permiso otorgado", "OK");
        }
        else if (status == PermissionStatus.Denied)
        {
            await DisplayAlert("Aviso", "Permiso denegado", "OK");
        }
    }

    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        var sitio = new ModeloSQL.Sitios
        {
            Imagen = GetImage64(),
            latitud = txtLatitud.Text,
            longitud = txtLongitud.Text,
            descripcion = txtDescripcion.Text
        };

        if (validar() == true)
        {
            if (await sitiosBD.StoreSitio(sitio) > 0) //
            {
                await DisplayAlert("Aviso", "Sitio ingresado con exito", "OK");
            }
            else
            {
                await DisplayAlert("Aviso", "No se puede insertar la información", "OK");
            }
        }     

    }
    //Metodo para validar campos vacios
    public Boolean validar()
    {
        Boolean campoVacio = true;
        if(string.IsNullOrEmpty(txtLatitud.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de Latitud vacío", "OK");
        }else if(string.IsNullOrEmpty(txtLongitud.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de Longitud vacío", "OK");
        }
        else if (string.IsNullOrEmpty(txtDescripcion.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de descripcion vacío", "OK");
        }else if(photo == null)
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Imagen del Sitio Vacía", "OK");
        }

        return campoVacio;
    }
    private async void btnLista_Clicked(object sender, EventArgs e)
    {
        var items = await sitiosBD.GetListSitios();
        await Navigation.PushAsync(new Vistas.SelectSitios(items));
    }

    private void btnSalir_Clicked(object sender, EventArgs e)
    {
    }

    private async void btnFoto_Clicked(object sender, EventArgs e)
    {
        photo = await MediaPicker.CapturePhotoAsync(); //Para capturar la foto se usa esa función asincrona

        if (photo != null)
        {
            //Path para guardar la foto
            string path = Path.Combine(FileSystem.CacheDirectory, photo.FileName); // photo.FileName es el nombre del archivo
            using Stream sourcephoto = await photo.OpenReadAsync(); //Abrir la foto o leerla.
            using FileStream StreamLocal = File.OpenWrite(path);

            //Mostrar el archivo de imagen en el objeto de image
            foto.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result);

            await sourcephoto.CopyToAsync(StreamLocal); //Guardando localmente la fotografía.
        }

    }

    //Pasar imagen a Base64
    public String GetImage64()
    {
        if (photo != null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Stream stream = photo.OpenReadAsync().Result;
                stream.CopyTo(ms);
                byte[] data = ms.ToArray(); //arreglo de bytes que esta en memoria

                String Base64 = Convert.ToBase64String(data); //aqui convertimos la imagen a base 64.
                return Base64;
            }
        }
        return null;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var connection = Connectivity.NetworkAccess;
        var local = CrossGeolocator.Current;

        if (connection == NetworkAccess.Internet)
        {
            if (local == null || !local.IsGeolocationAvailable || !local.IsGeolocationEnabled)
            {
                // Si la geolocalización no está disponible o no está habilitada
                CheckAndRequestLocationPermissionAsync();
            }
            else
            {
                GetLocationAsync();
            }
        }
        else
        {
            await DisplayAlert("Sin Acceso a internet", "Por favor, revisa tu conexion a internet para continuar.", "OK");
        }
    }
}