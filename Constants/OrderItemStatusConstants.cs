namespace FoodioAPI.Constants
{
    public static class OrderItemStatusConstants
    {
        public static readonly Guid PENDING = Guid.Parse("9f9d25c6-f53c-460d-99c0-ac76e015249e");
        public static readonly Guid COOKING = Guid.Parse("0a010f94-0ef0-449b-b84a-ecd9d0a8be34");
        public static readonly Guid CONFIRMED = Guid.Parse("47e3a54c-17a9-440e-8632-4ba37632830a");
        public static readonly Guid READY_TO_SERVE = Guid.Parse("736ddc8e-5c6b-45ab-b97a-f9f4117b8266");
        public static readonly Guid SERVED = Guid.Parse("848acf14-63dc-4e48-89b1-2f84f37f2c5b");
        public static readonly Guid COMPLETED = Guid.Parse("a58dbf33-ee0c-404f-bbec-ef7e2924e1d9");
        public static readonly Guid CANCELLED = Guid.Parse("f92f7b54-56d2-4a7a-b384-c417ad66f154");
    }
}
