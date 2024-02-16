using PM2E1393472.Controles;
using PM2E1393472.ModeloSQL;

namespace PM2E1393472.Vistas;

public partial class UpdateSitios : ContentPage
{
    private Controles.SitiosControl sitiosBD;
    FileResult photo; //Objeto Global
    Sitios new_sitios = new Sitios();

    public UpdateSitios()
    {
        InitializeComponent();

        sitiosBD = new Controles.SitiosControl();
        //Permisos de Geolocalizacion
        var connection = Connectivity.Current.NetworkAccess;
        if (connection == NetworkAccess.Internet)
        {
            try
            {
                var location = Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });

                if (location != null)
                {
                    // La geolocalización está disponible y habilitada
                    GetLocationAsync();
                }
                else
                {
                    // La geolocalización no está disponible
                    //permisosGeolocalizacionAsync();
                }
            }
            catch (FeatureNotSupportedException)
            {
                // La geolocalización no es compatible en este dispositivo
                // Manejar la excepción
            }
            catch (FeatureNotEnabledException)
            {
                // La geolocalización no está habilitada
                // Solicitar al usuario que habilite la geolocalización
            }
            catch (PermissionException)
            {
                // Problema con los permisos
                // Solicitar o verificar los permisos
            }
            catch (Exception ex)
            {
                // Otras excepciones
                // Manejar la excepción
            }
        }
        else
        {
            DisplayAlert("Sin Acceso a Internet", "Por favor, revisa tu conexión a Internet para continuar.", "OK");
        }
        Console.WriteLine(this.BindingContext);
    }

    public UpdateSitios(Controles.SitiosControl rutaBD)
    {
        InitializeComponent();
        sitiosBD = rutaBD;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        new_sitios = (Sitios)this.BindingContext;
    }

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
            await DisplayAlert("Advertencia", ex + "", "OK");
        }
    }

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        var Datos = new ModeloSQL.Sitios
        {
            id = int.Parse(txtUpOculto.Text),
            Imagen = GetImage64(),
            latitud = txtLatitud.Text,
            longitud = txtLongitud.Text,
            descripcion = txtDescripcion.Text,
        };

        //Mandar los datos para hacer el respectivo de update de la tabla
        if (validar() == true)
        {
            if (await sitiosBD.StoreSitio(Datos) > 0)
            {
                await DisplayAlert("Aviso", "Registro actualizado con exito", "OK");
            }
            else
            {
                await DisplayAlert("Aviso", "No se puede actualizar la información", "OK");
            }
        }
    }
    public Boolean validar()
    {
        Boolean campoVacio = true;
        if (string.IsNullOrEmpty(txtLatitud.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de Latitud vacío", "OK");
        }
        else if (string.IsNullOrEmpty(txtLongitud.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de Longitud vacío", "OK");
        }
        else if (string.IsNullOrEmpty(txtDescripcion.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de descripcion vacío", "OK");
        }
        else if (foto.Source.IsEmpty)
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Imagen del Sitio Vacía", "OK");
        }

        return campoVacio;
    }

    private async void btnFotoAct_Clicked(object sender, EventArgs e)
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
        else
        {
            return new_sitios.Imagen.ToString();
        }
    }
}