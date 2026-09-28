using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public static class FakeDataFactory
    {
        public static List<PromoCode> PromoCodes
        {
            get 
            {                
                var today = DateTime.UtcNow;
                
                var promoCodes = new List<PromoCode>()
                {
                    new PromoCode()
                    {
                        Id = Guid.Parse("5a763f6e-6a3c-4c32-b29c-aab5400c7a99"),
                        Code = "MH123124",
                        ServiceInfo = "Билеты на лучший спектакль сезона",
                        BeginDate = today.AddDays(-5),
                        EndDate = today.AddDays(-30),
                        PartnerId = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
                        PreferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c")
                    },
                    new PromoCode()
                    {
                        Id = Guid.Parse("9c48e4c4-9778-44fe-acfa-34828f56a098"),
                        Code = "MH128901",
                        ServiceInfo = "Ёлка",
                        BeginDate = today.AddDays(-5),
                        EndDate = today.AddDays(-30),
                        PartnerId = Guid.Parse("894b6e9b-eb5f-406c-aefa-8ccb35d39319"),
                        PreferenceId = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84")
                    }
                };

                return promoCodes;
            }
        }

        public static List<Customer> Customers
        {
            get
            {
                var customerId1 = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
                var customerId2 = Guid.NewGuid();
                var customerId3 = Guid.NewGuid();
                var customers = new List<Customer>()
                {
                    new Customer()
                    {
                        Id = customerId1,
                        Email = "ivan_sergeev@mail.ru",
                        FirstName = "Иван",
                        LastName = "Петров",
                        Preferences = new List<CustomerPreference>()
                        {
                            new CustomerPreference()
                            {
                                CustomerId = customerId1,
                                PreferenceId = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84")
                            },
                            new CustomerPreference()
                            {
                                CustomerId = customerId1,
                                PreferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c")
                            }
                        }
                    },                    
                    new Customer()
                    {
                        Id = customerId2,
                        Email = "Alex@mail.ru",
                        FirstName = "Алексей",
                        LastName = "Винокуров",
                        Preferences = new List<CustomerPreference>()
                        {
                            new CustomerPreference()
                            {
                                CustomerId = customerId2,
                                PreferenceId = Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84")
                            }
                        },
                        PromoCodes = new List<PromoCodeCustomer>()
                        {
                            new PromoCodeCustomer()
                            {
                                PromoCodeId = Guid.Parse("5a763f6e-6a3c-4c32-b29c-aab5400c7a99"),
                                CustomerId = customerId2
                            },
                            new PromoCodeCustomer()
                            {
                                PromoCodeId = Guid.Parse("9c48e4c4-9778-44fe-acfa-34828f56a098"),
                                CustomerId = customerId2
                            }
                        }
                    },

                    new Customer()
                    {
                        Id = customerId3,
                        Email = "Bob@mail.ru",
                        FirstName = "Боб",
                        LastName = "Огурцов",
                        Preferences = new List<CustomerPreference>()
                        {
                            new CustomerPreference()
                            {
                                CustomerId = customerId3,
                                PreferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c")
                            }
                        }
                    }

                };

                return customers;
            }
        }
    }
}