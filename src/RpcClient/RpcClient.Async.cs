using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Network.RPC
{
    /// <summary>
    /// The RPC client to call NEO RPC methods
    /// </summary>
    public partial class RpcClient : IDisposable
    {
        private HttpClient httpClient;

        public RpcClient(string url, string rpcUser = default, string rpcPass = default)
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(url) };
            if (!string.IsNullOrEmpty(rpcUser) && !string.IsNullOrEmpty(rpcPass))
            {
                string token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{rpcUser}:{rpcPass}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            }
        }

        public RpcClient(HttpClient client)
        {
            httpClient = client;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient?.Dispose();
                }

                httpClient = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public async Task<RpcResponse> SendAsync(RpcRequest request)
        {
            var requestJson = request.ToJson().ToString();
            using var result = await httpClient.PostAsync(httpClient.BaseAddress, new StringContent(requestJson, Encoding.UTF8));
            var content = await result.Content.ReadAsStringAsync();
            var response = RpcResponse.FromJson(JObject.Parse(content));
            response.RawResponse = content;

            if (response.Error != null)
            {
                throw new RpcException(response.Error.Code, response.Error.Message);
            }

            return response;
        }

        public virtual async Task<JObject> RpcSendAsync(string method, params JObject[] paraArgs)
        {
            var request = new RpcRequest
            {
                Id = 1,
                JsonRpc = "2.0",
                Method = method,
                Params = paraArgs
            };
            var respones = await SendAsync(request);
            return respones.Result;
        }

        #region Blockchain

        /// <summary>
        /// Returns the hash of the tallest block in the main chain.
        /// </summary>
        public async Task<string> GetBestBlockHashAsync()
        {
            var result = await RpcSendAsync("getbestblockhash");
            return result.AsString();
        }

        /// <summary>
        /// Returns the hash of the tallest block in the main chain.
        /// The serialized information of the block is returned, represented by a hexadecimal string.
        /// </summary>
        public async Task<string> GetBlockHexAsync(string hashOrIndex)
        {
            if (int.TryParse(hashOrIndex, out int index))
            {
                return (await RpcSendAsync("getblock", index)).AsString();
            }
            return (await RpcSendAsync("getblock", hashOrIndex)).AsString();
        }

        /// <summary>
        /// Returns the hash of the tallest block in the main chain.
        /// </summary>
        public async Task<RpcBlock> GetBlockAsync(string hashOrIndex)
        {
            if (int.TryParse(hashOrIndex, out int index))
            {
                return RpcBlock.FromJson(await RpcSendAsync("getblock", index, true));
            }
            return RpcBlock.FromJson(await RpcSendAsync("getblock", hashOrIndex, true));
        }

        /// <summary>
        /// Gets the number of blocks in the main chain.
        /// </summary>
        public async Task<uint> GetBlockCountAsync()
        {
            return (uint)(await RpcSendAsync("getblockcount")).AsNumber();
        }

        /// <summary>
        /// Returns the hash value of the corresponding block, based on the specified index.
        /// </summary>
        public async Task<string> GetBlockHashAsync(int index)
        {
            return (await RpcSendAsync("getblockhash", index)).AsString();
        }

        /// <summary>
        /// Returns the corresponding block header information according to the specified script hash.
        /// </summary>
        public async Task<string> GetBlockHeaderHexAsync(string hashOrIndex)
        {
            if (int.TryParse(hashOrIndex, out int index))
            {
                return (await RpcSendAsync("getblockheader", index)).AsString();
            }
            return (await RpcSendAsync("getblockheader", hashOrIndex)).AsString();
        }

        /// <summary>
        /// Returns the corresponding block header information according to the specified script hash.
        /// </summary>
        public async Task<RpcBlockHeader> GetBlockHeaderAsync(string hashOrIndex)
        {
            if (int.TryParse(hashOrIndex, out int index))
            {
                return RpcBlockHeader.FromJson(await RpcSendAsync("getblockheader", index, true));
            }
            return RpcBlockHeader.FromJson(await RpcSendAsync("getblockheader", hashOrIndex, true));
        }

        /// <summary>
        /// Queries contract information, according to the contract script hash.
        /// </summary>
        public async Task<ContractState> GetContractStateAsync(string hash)
        {
            return RpcContractState.FromJson(await RpcSendAsync("getcontractstate", hash)).ContractState;
        }

        /// <summary>
        /// Obtains the list of unconfirmed transactions in memory.
        /// </summary>
        public async Task<string[]> GetRawMempoolAsync()
        {
            return ((JArray)await RpcSendAsync("getrawmempool")).Select(p => p.AsString()).ToArray();
        }

        /// <summary>
        /// Obtains the list of unconfirmed transactions in memory.
        /// shouldGetUnverified = true
        /// </summary>
        public async Task<RpcRawMemPool> GetRawMempoolBothAsync()
        {
            return RpcRawMemPool.FromJson(await RpcSendAsync("getrawmempool", true));
        }

        /// <summary>
        /// Returns the corresponding transaction information, based on the specified hash value.
        /// </summary>
        public async Task<string> GetRawTransactionHexAsync(string txHash)
        {
            return (await RpcSendAsync("getrawtransaction", txHash)).AsString();
        }

        /// <summary>
        /// Returns the corresponding transaction information, based on the specified hash value.
        /// verbose = true
        /// </summary>
        public async Task<RpcTransaction> GetRawTransactionAsync(string txHash)
        {
            return RpcTransaction.FromJson(await RpcSendAsync("getrawtransaction", txHash, true));
        }

        /// <summary>
        /// Returns the stored value, according to the contract script hash (or Id) and the stored key.
        /// </summary>
        public async Task<string> GetStorageAsync(string scriptHashOrId, string key)
        {
            if (int.TryParse(scriptHashOrId, out int id))
            {
                return (await RpcSendAsync("getstorage", id, key)).AsString();
            }

            return (await RpcSendAsync("getstorage", scriptHashOrId, key)).AsString();
        }

        /// <summary>
        /// Returns the block index in which the transaction is found.
        /// </summary>
        public async Task<uint> GetTransactionHeightAsync(string txHash)
        {
            return (uint)(await RpcSendAsync("gettransactionheight", txHash)).AsNumber();
        }

        /// <summary>
        /// Returns the current NEO consensus nodes information and voting status.
        /// </summary>
        public async Task<RpcValidator[]> GetValidatorsAsync()
        {
            return ((JArray)await RpcSendAsync("getvalidators")).Select(p => RpcValidator.FromJson(p)).ToArray();
        }

        #endregion Blockchain

        #region Node

        /// <summary>
        /// Gets the current number of connections for the node.
        /// </summary>
        public async Task<int> GetConnectionCountAsync()
        {
            return (int)(await RpcSendAsync("getconnectioncount")).AsNumber();
        }

        /// <summary>
        /// Gets the list of nodes that the node is currently connected/disconnected from.
        /// </summary>
        public async Task<RpcPeers> GetPeersAsync()
        {
            return RpcPeers.FromJson(await RpcSendAsync("getpeers"));
        }

        /// <summary>
        /// Returns the version information about the queried node.
        /// </summary>
        public async Task<RpcVersion> GetVersionAsync()
        {
            return RpcVersion.FromJson(await RpcSendAsync("getversion"));
        }

        /// <summary>
        /// Broadcasts a serialized transaction over the NEO network.
        /// </summary>
        public async Task<UInt256> SendRawTransactionAsync(byte[] rawTransaction)
        {
            return UInt256.Parse((await RpcSendAsync("sendrawtransaction", rawTransaction.ToHexString()))["hash"].AsString());
        }

        /// <summary>
        /// Broadcasts a transaction over the NEO network.
        /// </summary>
        public async Task<UInt256> SendRawTransactionAsync(Transaction transaction)
        {
            return await SendRawTransactionAsync(transaction.ToArray());
        }

        /// <summary>
        /// Broadcasts a serialized block over the NEO network.
        /// </summary>
        public async Task<UInt256> SubmitBlockAsync(byte[] block)
        {
            return UInt256.Parse((await RpcSendAsync("submitblock", block.ToHexString()))["hash"].AsString());
        }

        #endregion Node

        #region SmartContract

        /// <summary>
        /// Returns the result after calling a smart contract at scripthash with the given operation and parameters.
        /// This RPC call does not affect the blockchain in any way.
        /// </summary>
        public async Task<RpcInvokeResult> InvokeFunctionAsync(string scriptHash, string operation, RpcStack[] stacks)
        {
            return RpcInvokeResult.FromJson(await RpcSendAsync("invokefunction", scriptHash, operation, stacks.Select(p => p.ToJson()).ToArray()));
        }

        /// <summary>
        /// Returns the result after passing a script through the VM.
        /// This RPC call does not affect the blockchain in any way.
        /// </summary>
        public async Task<RpcInvokeResult> InvokeScriptAsync(byte[] script, params UInt160[] scriptHashesForVerifying)
        {
            List<JObject> parameters = new List<JObject>
            {
                script.ToHexString()
            };
            parameters.AddRange(scriptHashesForVerifying.Select(p => (JObject)p.ToString()));
            return RpcInvokeResult.FromJson(await RpcSendAsync("invokescript", parameters.ToArray()));
        }

        #endregion SmartContract

        #region Utilities

        /// <summary>
        /// Returns a list of plugins loaded by the node.
        /// </summary>
        public async Task<RpcPlugin[]> ListPluginsAsync()
        {
            return ((JArray)await RpcSendAsync("listplugins")).Select(p => RpcPlugin.FromJson(p)).ToArray();
        }

        /// <summary>
        /// Verifies that the address is a correct NEO address.
        /// </summary>
        public async Task<RpcValidateAddressResult> ValidateAddressAsync(string address)
        {
            return RpcValidateAddressResult.FromJson(await RpcSendAsync("validateaddress", address));
        }

        #endregion Utilities

        #region Wallet

        /// <summary>
        /// Close the wallet opened by RPC.
        /// </summary>
        public async Task<bool> CloseWalletAsync()
        {
            return (await RpcSendAsync("closewallet")).AsBoolean();
        }

        /// <summary>
        /// Exports the private key of the specified address.
        /// </summary>
        public async Task<string> DumpPrivKeyAsync(string address)
        {
            return (await RpcSendAsync("dumpprivkey", address)).AsString();
        }

        /// <summary>
        /// Returns the balance of the corresponding asset in the wallet, based on the specified asset Id.
        /// This method applies to assets that conform to NEP-5 standards.
        /// </summary>
        /// <returns>new address as string</returns>
        public async Task<BigDecimal> GetBalanceAsync(string assetId)
        {
            byte decimals = new Nep5API(this).Decimals(UInt160.Parse(assetId));
            BigInteger balance = BigInteger.Parse((await RpcSendAsync("getbalance", assetId))["balance"].AsString());
            return new BigDecimal(balance, decimals);
        }

        /// <summary>
        /// Creates a new account in the wallet opened by RPC.
        /// </summary>
        public async Task<string> GetNewAddressAsync()
        {
            return (await RpcSendAsync("getnewaddress")).AsString();
        }

        /// <summary>
        /// Gets the amount of unclaimed GAS in the wallet.
        /// </summary>
        public async Task<BigInteger> GetUnclaimedGasAsync()
        {
            return BigInteger.Parse((await RpcSendAsync("getunclaimedgas")).AsString());
        }

        /// <summary>
        /// Imports the private key to the wallet.
        /// </summary>
        public async Task<RpcAccount> ImportPrivKeyAsync(string wif)
        {
            return RpcAccount.FromJson(await RpcSendAsync("importprivkey", wif));
        }

        /// <summary>
        /// Lists all the accounts in the current wallet.
        /// </summary>
        public async Task<List<RpcAccount>> ListAddressAsync()
        {
            return ((JArray)await RpcSendAsync("listaddress")).Select(p => RpcAccount.FromJson(p)).ToList();
        }

        /// <summary>
        /// Open wallet file in the provider's machine.
        /// By default, this method is disabled by RpcServer config.json.
        /// </summary>
        public async Task<bool> OpenWalletAsync(string path, string password)
        {
            return (await RpcSendAsync("openwallet", path, password)).AsBoolean();
        }

        /// <summary>
        /// Transfer from the specified address to the destination address.
        /// </summary>
        /// <returns>This function returns Signed Transaction JSON if successful, ContractParametersContext JSON if signing failed.</returns>
        public async Task<JObject> SendFromAsync(string assetId, string fromAddress, string toAddress, string amount)
        {
            return await RpcSendAsync("sendfrom", assetId, fromAddress, toAddress, amount);
        }

        /// <summary>
        /// Bulk transfer order, and you can specify a sender address.
        /// </summary>
        /// <returns>This function returns Signed Transaction JSON if successful, ContractParametersContext JSON if signing failed.</returns>
        public async Task<JObject> SendManyAsync(string fromAddress, IEnumerable<RpcTransferOut> outputs)
        {
            var parameters = new List<JObject>();
            if (!string.IsNullOrEmpty(fromAddress))
            {
                parameters.Add(fromAddress);
            }
            parameters.Add(outputs.Select(p => p.ToJson()).ToArray());

            return await RpcSendAsync("sendmany", paraArgs: parameters.ToArray());
        }

        /// <summary>
        /// Transfer asset from the wallet to the destination address.
        /// </summary>
        /// <returns>This function returns Signed Transaction JSON if successful, ContractParametersContext JSON if signing failed.</returns>
        public async Task<JObject> SendToAddressAsync(string assetId, string address, string amount)
        {
            return await RpcSendAsync("sendtoaddress", assetId, address, amount);
        }

        #endregion Utilities

        #region Plugins

        /// <summary>
        /// Returns the contract log based on the specified txHash. The complete contract logs are stored under the ApplicationLogs directory.
        /// This method is provided by the plugin ApplicationLogs.
        /// </summary>
        public async Task<RpcApplicationLog> GetApplicationLogAsync(string txHash)
        {
            return RpcApplicationLog.FromJson(await RpcSendAsync("getapplicationlog", txHash));
        }

        /// <summary>
        /// Returns all the NEP-5 transaction information occurred in the specified address.
        /// This method is provided by the plugin RpcNep5Tracker.
        /// </summary>
        /// <param name="address">The address to query the transaction information.</param>
        /// <param name="startTimestamp">The start block Timestamp, default to seven days before UtcNow</param>
        /// <param name="endTimestamp">The end block Timestamp, default to UtcNow</param>
        public async Task<RpcNep5Transfers> GetNep5TransfersAsync(string address, ulong? startTimestamp = default, ulong? endTimestamp = default)
        {
            startTimestamp ??= 0;
            endTimestamp ??= DateTime.UtcNow.ToTimestampMS();
            return RpcNep5Transfers.FromJson(await RpcSendAsync("getnep5transfers", address, startTimestamp, endTimestamp));
        }

        /// <summary>
        /// Returns the balance of all NEP-5 assets in the specified address.
        /// This method is provided by the plugin RpcNep5Tracker.
        /// </summary>
        public async Task<RpcNep5Balances> GetNep5BalancesAsync(string address)
        {
            return RpcNep5Balances.FromJson(await RpcSendAsync("getnep5balances", address));
        }

        #endregion Plugins
    }
}
