namespace FashionPay.Application.Common;

public static class BusinessConstants
{
    public static class Client
    {
        public const decimal MAX_CREDIT_LIMIT = 100000;
        public const decimal MIN_CREDIT_LIMIT = 1000;
        public const int MAX_PAYMENTS = 60;
        public const int MIN_PAYMENTS = 1;
        public const int DEFAULT_MAX_PAYMENTS = 12;
        public const int DEFAULT_PAYMENT_PERIOD = 30;
        public const int DEFAULT_DELINQUENCY_TOLERANCE = 5;
    }

    public static class Product
    {
        public const int MIN_STOCK = 0;
        public const int MAX_STOCK = 9999;
        public const decimal MIN_PRICE = 0.01m;
        public const decimal MAX_PRICE = 999999.99m;
    }

    public static class Purchase
    {
        public const decimal MIN_PURCHASE_AMOUNT = 100.00m;
        public const int MIN_QUANTITY = 1;
        public const int MAX_QUANTITY = 9999;
    }

    public static class Payment
    {
        public const decimal MIN_PAYMENT_AMOUNT = 0.01m;
        public static readonly string[] VALID_PAYMENT_METHODS = { "EFECTIVO", "TRANSFERENCIA", "TARJETA" };
    }

    public static class Validation
    {
        public const int MIN_NAME_LENGTH = 2;
        public const int MAX_NAME_LENGTH = 100;
        public const int MIN_EMAIL_LENGTH = 5;
        public const int MAX_EMAIL_LENGTH = 100;
        public const int MIN_PHONE_LENGTH = 10;
        public const int MAX_PHONE_LENGTH = 20;
        public const int MIN_ADDRESS_LENGTH = 5;
        public const int MAX_ADDRESS_LENGTH = 200;
        public const int MIN_DESCRIPTION_LENGTH = 5;
        public const int MAX_DESCRIPTION_LENGTH = 500;
        public const int MIN_CODE_LENGTH = 3;
        public const int MAX_CODE_LENGTH = 50;
        public const int MIN_SEARCH_TERM_LENGTH = 2;
    }
}