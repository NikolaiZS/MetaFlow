using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase;

namespace MetaFlow.Infrastructure.Services.SupabaseServices
{
    public class SupabaseService
    {
        public Supabase.Client Client { get; }

        public SupabaseService(string url, string serviceRoleKey)
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };

            Client = new Supabase.Client(url, serviceRoleKey, options);
        }

        public async Task InitializeAsync()
        {
            await Client.InitializeAsync();
        }
    }


}
