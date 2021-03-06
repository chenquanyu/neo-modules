using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.Wallets;

namespace Neo.Network.RPC
{
    /// <summary>
    /// Contract related operations through RPC API
    /// </summary>
    public class ContractClient
    {
        protected readonly RpcClient rpcClient;

        /// <summary>
        /// ContractClient Constructor
        /// </summary>
        /// <param name="rpc">the RPC client to call NEO RPC methods</param>
        public ContractClient(RpcClient rpc)
        {
            rpcClient = rpc;
        }

        /// <summary>
        /// Use RPC method to test invoke operation.
        /// </summary>
        /// <param name="scriptHash">contract script hash</param>
        /// <param name="operation">contract operation</param>
        /// <param name="args">operation arguments</param>
        /// <returns></returns>
        public RpcInvokeResult TestInvoke(UInt160 scriptHash, string operation, params object[] args)
        {
            byte[] script = scriptHash.MakeScript(operation, args);
            return rpcClient.InvokeScript(script);
        }

        /// <summary>
        /// Deploy Contract, return signed transaction
        /// </summary>
        /// <param name="contractScript">contract script</param>
        /// <param name="manifest">contract manifest</param>
        /// <param name="key">sender KeyPair</param>
        /// <returns></returns>
        public Transaction CreateDeployContractTx(byte[] contractScript, ContractManifest manifest, KeyPair key)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall(InteropService.Contract.Create, contractScript, manifest.ToString());
                script = sb.ToArray();
            }

            UInt160 sender = Contract.CreateSignatureRedeemScript(key.PublicKey).ToScriptHash();
            Transaction tx = new TransactionManager(rpcClient, sender)
                .MakeTransaction(script, null, null)
                .AddSignature(key)
                .Sign()
                .Tx;

            return tx;
        }
    }
}
