const fileDir = "output.txt";
const bip39 = require('bip39');
const bip32 = require('bip32');
var bitcoin = require('bitcoinjs-lib');
var bitcoinNetwork = bitcoin.networks.bitcoin;

var readline = require('readline');

var lineReader = readline.createInterface({
  input: require('fs').createReadStream(fileDir)
});


const targetAddress = "3HX5tttedDehKWTTGpxaPAbo157fnjn89s";

let logToConsole = true;//input.toLowerCase().startsWith("n") ? false : true;

lineReader.on('line', function (line) {
  	let skip = !line.startsWith("army");

	if(skip){
		return;
	}
	
	const seed = bip39.mnemonicToSeedSync(line);

	const root = bip32.fromSeed(seed, bitcoinNetwork);
	var hdMaster = bitcoin.bip32.fromSeed(seed, bitcoinNetwork);
	var bip141key = hdMaster.derivePath("m/44'/0'/0'/0");
	var bip49key = hdMaster.derivePath("m/49'/0'/0'/0/0");
	
	var bip141Wif = bip141key.toWIF();
	var bip49Wif = bip49key.toWIF();	

 	const bip141KeyPair = bitcoin.ECPair.fromWIF(bip141Wif);
 	const bip49KeyPair = bitcoin.ECPair.fromWIF(bip49Wif);

    const bip141address = bitcoin.payments.p2sh({
      redeem: bitcoin.payments.p2wpkh({ pubkey: bip141KeyPair.publicKey })
  	}).address;

    const bip49address = bitcoin.payments.p2sh({
      redeem: bitcoin.payments.p2wpkh({ pubkey: bip49KeyPair.publicKey })
  	}).address;
    	  	
	if(bip141address == targetAddress){
		if(logToConsole){
		console.log(line);
		console.log(`^ Match found bip141 address: ${bip141address} publicKey: ${bip141KeyPair.publicKey.toString('hex')} privateKey: ${bip141Wif}}`);
		}
	}
	if(bip49address == targetAddress){
		if(logToConsole){
		console.log(line);
		console.log(`^ Match found bip49 address: ${bip49address} publicKey: ${bip49KeyPair.publicKey.toString('hex')} privateKey: ${bip49Wif}}`);
		}
	}
});