using HydroSales.Authorization;
using HydroSales.Database;
using HydroSales.Domain;

namespace HydroSales.Pages.Start;

public interface IMembershipService
{
    Task<User> SignUp();
}

public class MembershipService(IDatabase database, IAuthorizationService authorizationService) : IMembershipService
{
    public async Task<User> SignUp()
    {
        var user = User.Create(insertTestData: true);

        await CreateTestData(user);

        await database.AddAsync(user);
        await authorizationService.SignIn(user);
        await database.SaveChangesAsync();
        return user;
    }

    private async Task CreateTestData(User user)
    {
        var product1 = Product.Create(
            user: user,
            name: "UltraHD 4K Monitor",
            code: "UHD4K",
            priceNet: 599,
            salesTax: 10,
            currencyCode: "USD"
        );

        var product2 = Product.Create(
            user: user,
            name: "Wireless Noise Cancelling Headphones",
            code: "WNCH",
            priceNet: 349,
            salesTax: 10,
            currencyCode: "USD"
        );

        var product3 = Product.Create(
            user: user,
            name: "Professional DSLR Camera",
            code: "PDC123",
            priceNet: 1299,
            salesTax: 10,
            currencyCode: "USD"
        );

        var product4 = Product.Create(
            user: user,
            name: "Smart Fitness Watch",
            code: "SFW567",
            priceNet: 199,
            salesTax: 10,
            currencyCode: "USD"
        );

        var product5 = Product.Create(
            user: user,
            name: "Bluetooth Portable Speaker",
            code: "BPS981",
            priceNet: 99,
            salesTax: 10,
            currencyCode: "USD"
        );

        var customer1 = Customer.Create(
            user: user,
            name: "BrightTech Solutions",
            taxId: "US123456789",
            currencyCode: "USD",
            address: "200 Technology Square",
            city: "Cambridge",
            countryCode: "US",
            paymentTerms: 30
        );

        var customer2 = Customer.Create(
            user: user,
            name: "GreenLeaf Renewables",
            taxId: "GB987654321",
            currencyCode: "GBP",
            address: "15 Solar Rd",
            city: "London",
            countryCode: "EN",
            paymentTerms: 45
        );

        var customer3 = Customer.Create(
            user: user,
            name: "Oceanic Research Co.",
            taxId: "AU456789123",
            currencyCode: "AUD",
            address: "47 Ocean Street",
            city: "Sydney",
            countryCode: "AU",
            paymentTerms: 60
        );

        var customer4 = Customer.Create(
            user: user,
            name: "CyberSecure GmbH",
            taxId: "DE321654987",
            currencyCode: "EUR",
            address: "88 Digitalstraße",
            city: "Berlin",
            countryCode: "DE",
            paymentTerms: 20
        );

        var customer5 = Customer.Create(
            user: user,
            name: "SkyHigh Aviation",
            taxId: "FR123987456",
            currencyCode: "EUR",
            address: "33 Rue du Ciel",
            city: "Paris",
            countryCode: "FR",
            paymentTerms: 50
        );

        var invoice1 = CreateInvoice(user, customer3, new[] { product1, product2, product3 });
        var invoice2 = CreateInvoice(user, customer1, new[] { product2, product4 });

        await Add(product1, product2, product3, product4, product5);
        await Add(customer1, customer2, customer3, customer4, customer5);
        await Add(invoice1, invoice2);
    }

    private Invoice CreateInvoice(User user, Customer customer, IEnumerable<Product> products) =>
        Invoice.Create(
            user: user,
            customer: customer,
            currencyCode: customer.CurrencyCode,
            lines: products.Select(product1 => new InvoiceLine.Data(
                Id: null,
                Product: product1,
                CurrencyCode: product1.CurrencyCode,
                Unit: "pc",
                UnitPriceNet: product1.PriceNet,
                Quantity: new Random().Next(1, 4),
                SalesTax: product1.SalesTax
            )).ToList(),
            issueDate: DateTime.Today,
            dueDate: DateTime.Today.AddDays(customer.PaymentTerms),
            paymentTerms: customer.PaymentTerms,
            remarks: "Some notes"
        );

    private async Task Add(params IEntity[] entities)
    {
        foreach (var entity in entities)
        {
            await database.AddAsync(entity);
        }
    }
}