namespace PM2E1393472.Vistas;

public partial class UpdateSitios : ContentPage
{
    private Controles.SitiosControl sitiosBD;
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

    }

    public UpdateSitios(Controles.SitiosControl rutaBD)
    {
        InitializeComponent();
        sitiosBD = rutaBD;

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
            latitud = txtLatitud.Text,
            longitud = txtLongitud.Text,
            descripcion = txtDescripcion.Text,
        };

        //Mandar los datos para hacer el respectivo de update de la tabla

        if (await sitiosBD.StoreSitio(Datos) > 0) //
        {
            await DisplayAlert("Aviso", "Registro actualizado con exito", "OK");
        }
        else
        {
            await DisplayAlert("Aviso", "No se puede actualizar la información", "OK");
        }

    }
}