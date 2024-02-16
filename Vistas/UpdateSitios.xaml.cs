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
                    // La geolocalizaci�n est� disponible y habilitada
                    GetLocationAsync();
                }
                else
                {
                    // La geolocalizaci�n no est� disponible
                    //permisosGeolocalizacionAsync();
                }
            }
            catch (FeatureNotSupportedException)
            {
                // La geolocalizaci�n no es compatible en este dispositivo
                // Manejar la excepci�n
            }
            catch (FeatureNotEnabledException)
            {
                // La geolocalizaci�n no est� habilitada
                // Solicitar al usuario que habilite la geolocalizaci�n
            }
            catch (PermissionException)
            {
                // Problema con los permisos
                // Solicitar o verificar los permisos
            }
            catch (Exception ex)
            {
                // Otras excepciones
                // Manejar la excepci�n
            }
        }
        else
        {
            DisplayAlert("Sin Acceso a Internet", "Por favor, revisa tu conexi�n a Internet para continuar.", "OK");
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
            await DisplayAlert("Aviso", "No se puede actualizar la informaci�n", "OK");
        }

    }
}