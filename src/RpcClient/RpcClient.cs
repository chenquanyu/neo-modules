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
    partial class RpcClient
    {
        #region Blockchain

        /// <summary>
        /// Returns the hash of the tallest block in the main chain.
        /// </summary>
        public string GetBestBlockHash()
        {
            return GetBestBlockHashAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the hash of the tallest block in the main chain.
        /// The serialized information of the block is returned, represented by a hexadecimal string.
        /// </summary>
        public string GetBlockHex(string hashOrIndex)
        {
            return GetBlockHexAsync(hashOrIndex).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the hash of the tallest block in the main chain.
        /// </summary>
        public RpcBlock GetBlock(string hashOrIndex)
        {
            return GetBlockAsync(hashOrIndex).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the number of blocks in the main chain.
        /// </summary>
        public uint GetBlockCount()
        {
            return GetBlockCountAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the hash value of the corresponding block, based on the specified index.
        /// </summary>
        public string GetBlockHash(int index)
        {
            return GetBlockHashAsync(index).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the corresponding block header information according to the specified script hash.
        /// </summary>
        public string GetBlockHeaderHex(string hashOrIndex)
        {
            return GetBlockHeaderHexAsync(hashOrIndex).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the corresponding block header information according to the specified script hash.
        /// </summary>
        public RpcBlockHeader GetBlockHeader(string hashOrIndex)
        {
            return GetBlockHeaderAsync(hashOrIndex).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Queries contract information, according to the contract script hash.
        /// </summary>
        public ContractState GetContractState(string hash)
        {
            return GetContractStateAsync(hash).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtains the list of unconfirmed transactions in memory.
        /// </summary>
        public string[] GetRawMempool()
        {
            return GetRawMempoolAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtains the list of unconfirmed transactions in memory.
        /// shouldGetUnverified = true
        /// </summary>
        public RpcRawMemPool GetRawMempoolBoth()
        {
            return GetRawMempoolBothAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the corresponding transaction information, based on the specified hash value.
        /// </summary>
        public string GetRawTransactionHex(string txHash)
        {
            return GetRawTransactionHexAsync(txHash).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the corresponding transaction information, based on the specified hash value.
        /// verbose = true
        /// </summary>
        public RpcTransaction GetRawTransaction(string txHash)
        {
            return GetRawTransactionAsync(txHash).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the stored value, according to the contract script hash (or Id) and the stored key.
        /// </summary>
        public string GetStorage(string scriptHashOrId, string key)
        {
            return GetStorageAsync(scriptHashOrId, key).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the block index in which the transaction is found.
        /// </summary>
        public uint GetTransactionHeight(string txHash)
        {
            return GetTransactionHeightAsync(txHash).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the current NEO consensus nodes information and voting status.
        /// </summary>
        public RpcValidator[] GetValidators()
        {
            return GetValidatorsAsync().GetAwaiter().GetResult();
        }

        #endregion Blockchain

        #region Node

        /// <summary>
        /// Gets the current number of connections for the node.
        /// </summary>
        public int GetConnectionCount()
        {
            return GetConnectionCountAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the list of nodes that the node is currently connected/disconnected from.
        /// </summary>
        public RpcPeers GetPeers()
        {
            return GetPeersAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the version information about the queried node.
        /// </summary>
        public RpcVersion GetVersion()
        {
            return GetVersionAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Broadcasts a serialized transaction over the NEO network.
        /// </summary>
        public UInt256 SendRawTransaction(byte[] rawTransaction)
        {
            return SendRawTransactionAsync(rawTransaction).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Broadcasts a transaction over the NEO network.
        /// </summary>
        public UInt256 SendRawTransaction(Transaction transaction)
        {
            return SendRawTransactionAsync(transaction).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Broadcasts a serialized block over the NEO network.
        /// </summary>
        public UInt256 SubmitBlock(byte[] block)
        {
            return SubmitBlockAsync(block).GetAwaiter().GetResult();
        }

        #endregion Node

        #region SmartContract

        /// <summary>
        /// Returns the result after calling a smart contract at scripthash with the given operation and parameters.
        /// This RPC call does not affect the blockchain in any way.
        /// </summary>
        public RpcInvokeResult InvokeFunction(string scriptHash, string operation, RpcStack[] stacks)
        {
            return InvokeFunctionAsync(scriptHash, operation, stacks).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the result after passing a script through the VM.
        /// This RPC call does not affect the blockchain in any way.
        /// </summary>
        public RpcInvokeResult InvokeScript(byte[] script, params UInt160[] scriptHashesForVerifying)
        {
            return InvokeScriptAsync(script, scriptHashesForVerifying).GetAwaiter().GetResult();
        }

        #endregion SmartContract

        #region Utilities

        /// <summary>
        /// Returns a list of plugins loaded by the node.
        /// </summary>
        public RpcPlugin[] ListPlugins()
        {
            return ListPluginsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Verifies that the address is a correct NEO address.
        /// </summary>
        public RpcValidateAddressResult ValidateAddress(string address)
        {
            return ValidateAddressAsync(address).GetAwaiter().GetResult();
        }

        #endregion Utilities

        #region Wallet

        /// <summary>
        /// Close the wallet opened by RPC.
        /// </summary>
        public bool CloseWallet()
        {
            return CloseWalletAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Exports the private key of the specified address.
        /// </summary>
        public string DumpPrivKey(string address)
        {
            return DumpPrivKeyAsync(address).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the balance of the corresponding asset in the wallet, based on the specified asset Id.
        /// This method applies to assets that conform to NEP-5 standards.
        /// </summary>
        /// <returns>new address as string</returns>
        public BigDecimal GetBalance(string assetId)
        {
            return GetBalanceAsync(assetId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a new account in the wallet opened by RPC.
        /// </summary>
        public string GetNewAddress()
        {
            return GetNewAddressAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the amount of unclaimed GAS in the wallet.
        /// </summary>
        public BigInteger GetUnclaimedGas()
        {
            return GetUnclaimedGasAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Imports the private key to the wallet.
        /// </summary>
        public RpcAccount ImportPrivKey(string wif)
        {
            return ImportPrivKeyAsync(wif).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Lists all the accounts in the current wallet.
        /// </summary>
        public List<RpcAccount> ListAddress()
        {
            return ListAddressAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Open wallet file in the provider's machine.
        /// By default, this method is disabled by RpcServer config.json.
        /// </summary>
        public bool OpenWallet(string path, string password)
        {
            return OpenWalletAsync(path, password).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Transfer from the specified address to the destination address.
        /// </summary>
        /// <returns>This function returns Signed Transaction JSON if successful, ContractParametersContext JSON if signing failed.</returns>
        public JObject SendFrom(string assetId, string fromAddress, string toAddress, string amount)
        {
            return SendFromAsync(assetId, fromAddress, toAddress, amount).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Bulk transfer order, and you can specify a sender address.
        /// </summary>
        /// <returns>This function returns Signed Transaction JSON if successful, ContractParametersContext JSON if signing failed.</returns>
        public JObject SendMany(string fromAddress, IEnumerable<RpcTransferOut> outputs)
        {
            return SendManyAsync(fromAddress, outputs).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Transfer asset from the wallet to the destination address.
        /// </summary>
        /// <returns>This function returns Signed Transaction JSON if successful, ContractParametersContext JSON if signing failed.</returns>
        public JObject SendToAddress(string assetId, string address, string amount)
        {
            return SendToAddressAsync(assetId, address, amount).GetAwaiter().GetResult();
        }

        #endregion Utilities

        #region Plugins

        /// <summary>
        /// Returns the contract log based on the specified txHash. The complete contract logs are stored under the ApplicationLogs directory.
        /// This method is provided by the plugin ApplicationLogs.
        /// </summary>
        public RpcApplicationLog GetApplicationLog(string txHash)
        {
            return GetApplicationLogAsync(txHash).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns all the NEP-5 transaction information occurred in the specified address.
        /// This method is provided by the plugin RpcNep5Tracker.
        /// </summary>
        /// <param name="address">The address to query the transaction information.</param>
        /// <param name="startTimestamp">The start block Timestamp, default to seven days before UtcNow</param>
        /// <param name="endTimestamp">The end block Timestamp, default to UtcNow</param>
        public RpcNep5Transfers GetNep5Transfers(string address, ulong? startTimestamp = default, ulong? endTimestamp = default)
        {
            return GetNep5TransfersAsync(address, startTimestamp, endTimestamp).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the balance of all NEP-5 assets in the specified address.
        /// This method is provided by the plugin RpcNep5Tracker.
        /// </summary>
        public RpcNep5Balances GetNep5Balances(string address)
        {
            return GetNep5BalancesAsync(address).GetAwaiter().GetResult();
        }

        #endregion Plugins
    }
}
