namespace ReservasApi.Modelos
{
    public enum MetodoPago
    {
        Tarjeta, // sea de crédito o de débito
        Transferencia, // bancaria
        Otro // por ejemplo Bizum o PayPal. Podríamos desglosar esta última instancia en varias.
    }
}
