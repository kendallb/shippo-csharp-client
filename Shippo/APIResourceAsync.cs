/*
 * Copyright 2011 - 2012 Xamarin, Inc., 2011 - 2012 Joe Dluzen
 *
 * Author(s):
 *  Gonzalo Paniagua Javier (gonzalo@xamarin.com)
 *  Joe Dluzen (jdluzen@gmail.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shippo {
    public partial class APIResource {
        #region Shared

        // GET Requests
        public virtual async Task<T> DoRequestAsync<T>(string endpoint, string method = "GET", string body = null)
        {
            var json = await DoRequestAsync(endpoint, method, body);
            return JsonConvert.DeserializeObject<T>(json);
        }

        // GET Requests Helper
        public virtual Task<string> DoRequestAsync(string endpoint)
        {
            return DoRequestAsync(endpoint, "GET", null);
        }

        // Requests Main Function
        public virtual async Task<string> DoRequestAsync(string endpoint, string method, string body)
        {
            string result = null;
            WebRequest req = SetupRequest(method, endpoint);
            if (body != null) {
                byte[] bytes = encoding.GetBytes(body.ToString());
                req.ContentLength = bytes.Length;
                using (Stream st = req.GetRequestStream()) {
                    st.Write(bytes, 0, bytes.Length);
                }
            }

            try {
                using (var resp = await Task.Factory.FromAsync(req.BeginGetResponse, req.EndGetResponse, null)) {
                    result = GetResponseAsString(resp);
                }
            } catch (WebException wexc) {
                if (wexc.Response != null) {
                    string json_error = GetResponseAsString(wexc.Response);
                    HttpStatusCode status_code = HttpStatusCode.BadRequest;
                    HttpWebResponse resp = wexc.Response as HttpWebResponse;
                    if (resp != null)
                        status_code = resp.StatusCode;

                    if ((int)status_code <= 500)
                        throw new ShippoException(json_error, wexc);
                }
                throw;
            }
            return result;
        }

        #endregion

        #region Address

        public Task<Address> CreateAddressAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/addresses", api_endpoint);
            return DoRequestAsync<Address> (ep, "POST", serialize (parameters));
        }

        public Task<Address> RetrieveAddressAsync (String id)
        {
            string ep = String.Format ("{0}/addresses/{1}", api_endpoint, id);
            return DoRequestAsync<Address> (ep, "GET");
        }

        public Task<Address> ValidateAddressAsync(String id)
        {
            string ep = String.Format ("{0}/addresses/{1}/validate", api_endpoint, id);
            return DoRequestAsync<Address> (ep, "GET");
        }

        public Task<ShippoCollection<Address>> AllAddresssAsync(Hashtable parameters)
        {
            string ep = String.Format ("{0}/addresses?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Address>> (ep);
        }

        #endregion

        #region Parcel

        public Task<Parcel> CreateParcelAsync(Hashtable parameters)
        {
            string ep = String.Format ("{0}/parcels", api_endpoint);
            return DoRequestAsync<Parcel> (ep, "POST", serialize (parameters));
        }

        public Task<Parcel> RetrieveParcelAsync(String id)
        {
            string ep = String.Format ("{0}/parcels/{1}", api_endpoint, id);
            return DoRequestAsync<Parcel> (ep, "GET");
        }

        public Task<ShippoCollection<Parcel>> AllParcelsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/parcels?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Parcel>> (ep);
        }

        #endregion

        #region Shipment

        public Task<Shipment> CreateShipmentAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/shipments", api_endpoint);
            return DoRequestAsync<Shipment> (ep, "POST", serialize (parameters));
        }

        public Task<Shipment> RetrieveShipmentAsync (String id)
        {
            string ep = String.Format ("{0}/shipments/{1}", api_endpoint, id);
            return DoRequestAsync<Shipment> (ep, "GET");
        }

        public Task<ShippoCollection<Shipment>> AllShipmentsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/shipments?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Shipment>> (ep);
        }

        #endregion

        #region Rate

        public Task<ShippoCollection<Rate>> CreateRateAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/shipments/{1}/rates/{2}", api_endpoint, parameters ["id"], parameters ["currency_code"]);
            return DoRequestAsync<ShippoCollection<Rate>> (ep, "GET");
        }

        public Task<ShippoCollection<Rate>> GetShippingRatesSyncAsync (String objectId)
        {
            Hashtable parameters = new Hashtable ();
            parameters.Add ("id", objectId);
            parameters.Add ("currency_code", "");
            return GetShippingRatesSyncAsync (parameters);
        }

        public Task<ShippoCollection<Rate>> GetShippingRatesSyncAsync (Hashtable parameters)
        {

            String object_id = (String) parameters ["id"];
            Shipment shipment = RetrieveShipment (object_id);
            String object_status = (String) shipment.ObjectStatus;
            long startTime = DateTimeExtensions.UnixTimeNow ();

            while (object_status.Equals ("QUEUED", StringComparison.OrdinalIgnoreCase) || object_status.Equals ("WAITING", StringComparison.OrdinalIgnoreCase)) {
                if (DateTimeExtensions.UnixTimeNow () - startTime > RatesReqTimeout) {
                    throw new RequestTimeoutException (
                        "A timeout has occured while waiting for your rates to generate. Try retreiving the Shipment object again and check if object_status is updated. If this issue persists, please contact support@goshippo.com");
                }
                shipment = RetrieveShipment (object_id);
                object_status = (String) shipment.ObjectStatus;
            }

            return CreateRateAsync(parameters);
        }

        public Task<Rate> RetrieveRateAsync (String id)
        {
            string ep = String.Format ("{0}/rates/{1}", api_endpoint, id);
            return DoRequestAsync<Rate> (ep, "GET");
        }

        public Task<ShippoCollection<Rate>> AllRatesAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/rates?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Rate>> (ep);
        }

        #endregion

        #region Transaction

        public Task<Transaction> CreateTransactionAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/transactions", api_endpoint);
            return DoRequestAsync<Transaction> (ep, "POST", serialize (parameters));
        }

        public async Task<Transaction> CreateTransactionSyncAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/transactions", api_endpoint);
            Transaction transaction = await DoRequestAsync<Transaction> (ep, "POST", serialize (parameters));
            String object_id = (String) transaction.ObjectId;
            String object_status = (String) transaction.ObjectStatus;
            long startTime = DateTimeExtensions.UnixTimeNow ();

            while (object_status.Equals ("QUEUED", StringComparison.OrdinalIgnoreCase) || object_status.Equals ("WAITING", StringComparison.OrdinalIgnoreCase)) {
                if (DateTimeExtensions.UnixTimeNow () - startTime > TransactionReqTimeout) {
                    throw new RequestTimeoutException (
                        "A timeout has occured while waiting for your label to generate. Try retreiving the Transaction object again and check if object_status is updated. If this issue persists, please contact support@goshippo.com");
                }
                transaction = RetrieveTransaction (object_id);
                object_status = (String) transaction.ObjectStatus;
            }

            return transaction;
        }

        public Task<Transaction> RetrieveTransactionAsync (String id)
        {
            string ep = String.Format ("{0}/transactions/{1}", api_endpoint, id);
            return DoRequestAsync<Transaction> (ep, "GET");
        }

        public Task<ShippoCollection<Transaction>> AllTransactionsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/transactions?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Transaction>> (ep);
        }

        #endregion

        #region CustomsItem

        public Task<CustomsItem> CreateCustomsItemAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/customs/items", api_endpoint);
            return DoRequestAsync<CustomsItem> (ep, "POST", serialize (parameters));
        }

        public Task<CustomsItem> RetrieveCustomsItemAsync (String id)
        {
            string ep = String.Format ("{0}/customs/items/{1}", api_endpoint, id);
            return DoRequestAsync<CustomsItem> (ep, "GET");
        }

        public Task<ShippoCollection<CustomsItem>> AllCustomsItemsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/customs/items?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<CustomsItem>> (ep);
        }

        #endregion

        #region CustomsDeclaration

        public Task<CustomsDeclaration> CreateCustomsDeclarationAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/customs/declarations", api_endpoint);
            return DoRequestAsync<CustomsDeclaration> (ep, "POST", serialize (parameters));
        }

        public Task<CustomsDeclaration> RetrieveCustomsDeclarationAsync (String id)
        {
            string ep = String.Format ("{0}/customs/declarations/{1}", api_endpoint, id);
            return DoRequestAsync<CustomsDeclaration> (ep, "GET");
        }

        public Task<ShippoCollection<CustomsDeclaration>> AllCustomsDeclarationsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/customs/declarations?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<CustomsDeclaration>> (ep);
        }

        #endregion

        #region CarrierAccount

        public Task<CarrierAccount> CreateCarrierAccountAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/carrier_accounts", api_endpoint);
            return DoRequestAsync<CarrierAccount> (ep, "POST", serialize (parameters));
        }

        public Task<CarrierAccount> UpdateCarrierAccountAsync (String object_id, Hashtable parameters)
        {
            string ep = String.Format ("{0}/carrier_accounts/{1}", api_endpoint, object_id);
            return DoRequestAsync<CarrierAccount> (ep, "PUT", serialize (parameters));
        }

        public Task<CarrierAccount> RetrieveCarrierAccountAsync (String object_id)
        {
            string ep = String.Format ("{0}/carrier_accounts/{1}", api_endpoint, object_id);
            return DoRequestAsync<CarrierAccount> (ep, "GET");
        }

        public Task<ShippoCollection<CarrierAccount>> AllCarrierAccountAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/carrier_accounts?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<CarrierAccount>> (ep);
        }

        #endregion

        #region Refund

        public Task<Refund> CreateRefundAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/refunds", api_endpoint);
            return DoRequestAsync<Refund> (ep, "POST", serialize (parameters));
        }

        public Task<Refund> RetrieveRefundAsync (String id)
        {
            string ep = String.Format ("{0}/refunds/{1}", api_endpoint, id);
            return DoRequestAsync<Refund> (ep, "GET");
        }

        public Task<ShippoCollection<Refund>> AllRefundsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/refunds?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Refund>> (ep);
        }

        #endregion

        #region Manifest

        public Task<Manifest> CreateManifestAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/manifests", api_endpoint);
            return DoRequestAsync<Manifest> (ep, "POST", serialize (parameters));
        }

        public Task<Manifest> RetrieveManifestAsync (String id)
        {
            string ep = String.Format ("{0}/manifests/{1}", api_endpoint, id);
            return DoRequestAsync<Manifest> (ep, "GET");
        }

        public Task<ShippoCollection<Manifest>> AllManifestsAsync (Hashtable parameters)
        {
            string ep = String.Format ("{0}/manifests?{1}", api_endpoint, generateURLEncodedFromHashmap (parameters));
            return DoRequestAsync<ShippoCollection<Manifest>> (ep);
        }

        #endregion
    }
}
