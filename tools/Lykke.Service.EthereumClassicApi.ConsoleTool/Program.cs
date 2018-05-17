using Common.Log;
using Lykke.Service.EthereumClassicApi.Blockchain;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSignFacade.Contract.Models;
using Lykke.Service.BlockchainSignService.Client.Models;

namespace Lykke.Service.EthereumClassicApi.ConsoleTool
{
    class Program
    {
        private const string BlockchainType = "EthereumClassic";
        private static Lykke.Service.BlockchainSignFacade.Client.BlockchainSignFacadeClient _signServiceClient;
        private static IEthereum _ethereum;
        private const string SignServiceUrl = "signServiceUrl";
        private const string ApiKey = "apiKey";
        private const string EthereumNodeUrl = "ethereumNodeUrl";

        private static void Main(string[] args)
        {
            var application = new CommandLineApplication
            {
                Description = "Send all eth from typed ETC deposit address in ETH Network."
            };

            var arguments = new Dictionary<string, CommandArgument>
            {
                { SignServiceUrl, application.Argument(SignServiceUrl, "Url of a blockchain sign service facade(common).") },
                { ApiKey, application.Argument(ApiKey, "Api key of a blockchain integration sign facade(ETC).") },
                { EthereumNodeUrl, application.Argument(EthereumNodeUrl, "Ethereum Node Url(ETH Network). Parity expected.") }
            };

            application.HelpOption("-? | -h | --help");
            application.OnExecute(async () =>
            {
                try
                {
                    if (arguments.Any(x => string.IsNullOrEmpty(x.Value.Value)))
                    {
                        application.ShowHelp();
                    }
                    else
                    {
                        await TransferFromEtcDeposits
                        (
                            arguments[SignServiceUrl].Value,
                            arguments[ApiKey].Value,
                            arguments[EthereumNodeUrl].Value
                        );
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);

                    return 1;
                }
            });

            application.Execute(args);
        }


        private static void ListAllCommands()
        {
            Console.WriteLine("Commands List");
            Console.WriteLine("Press 1 - to transfer from ETC deposit address to the destination in ETH network");
        }

        private static async Task TransferFromEtcDeposits(string signServiceUrl, string apiKey, string ethNodeUrl)
        {
            var parity = new Nethereum.Parity.Web3Parity(ethNodeUrl);
            _ethereum = new Parity(parity);
            _signServiceClient = new Lykke.Service.BlockchainSignFacade.Client.BlockchainSignFacadeClient(signServiceUrl, apiKey, new LogToConsole());

            do
            {
                var etcDepositAddress = GetUserInputWithWalidation("Enter ETC Deposit Address(FROM)", "Address is not valid!",
                    (address) =>
                    {
                        var isValid = ValidateAddressAsync(address).Result;

                        return (isValid, isValid ? address : "Address is not a valid deposit address");
                    });

                var ethDepositAddress = GetUserInputWithWalidation("Enter ETH user's private wallet Address(TO)", "Address is not valid!",
                    (address) =>
                    {
                        var isValid = ValidateAddressAsync(address).Result;

                        return (isValid, isValid ? address : "Address is not a valid deposit address");
                    });

                var operationId = Guid.NewGuid();
                var balance = await _ethereum.GetLatestBalanceAsync(etcDepositAddress);
                var gasPrice = await parity.Eth.GasPrice.SendRequestAsync();
                var fee = gasPrice * Constants.EtcTransferGasAmount;
                var amount = balance - fee;

                if (amount <= 0)
                {
                    Console.WriteLine($"Address balance is no enough for transaction.(Fee - {fee}, Actual - {balance})");

                    return;
                }

                var nonce = await _ethereum.GetNextNonceAsync(etcDepositAddress);

                Console.WriteLine("Building Transaction");
                var buildedTransaction = _ethereum.BuildTransaction(
                    ethDepositAddress,
                    amount,
                    nonce,
                    gasPrice,
                    Constants.EtcTransferGasAmount);

                Console.WriteLine("Signing Transaction");
                var currentWallet = await _signServiceClient.GetWalletByPublicAddressAsync(BlockchainType, etcDepositAddress);

                if (currentWallet == null)
                {
                    Console.WriteLine("Deposit address does not exist");
                    continue;
                }

                var signedTransaction = await _signServiceClient.SignTransactionAsync(BlockchainType, new SignTransactionRequest()
                {
                    PublicAddresses = new[] { etcDepositAddress },
                    TransactionContext = buildedTransaction

                });

                Console.WriteLine("Sending Transaction");
                string trHex = await _ethereum.SendRawTransactionAsync(signedTransaction.SignedTransaction);

                Console.WriteLine(trHex + " was sent to " + ethDepositAddress);

                var @continue = GetUserInputWithWalidation("Want to make more transfers?(Y/N)", "(Y/N)?", (ack) =>
                {
                    if (ack.ToLower() == "y")
                        return (true, true);
                    if (ack.ToLower() == "n")
                        return (true, false);

                    return (false, false);
                });

                if (!@continue)
                    break;
            }
            while (true);
        }

        private static T GetUserInputWithWalidation<T>(string typeOfInput,
            string messageOnWrongInput,
            Func<string, (bool IsValid, T Result)> validFunc)
        {
            string input = "";

            do
            {
                Console.Write($"{typeOfInput}: ");
                input = Console.ReadLine();
                var validationResult = validFunc(input);

                if (validationResult.IsValid)
                {
                    return validationResult.Result;
                }

                Console.WriteLine($"Try Again! Error: {validationResult.Result.ToString()}");

            } while (true);
        }

        public static async Task<bool> ValidateAddressAsync(string address)
        {
            if (await AddressValidator.ValidateAsync(address))
            {
                var addressCode = await _ethereum.GetCodeAsync(address);

                return addressCode == "0x";
            }

            return false;
        }
    }
}
