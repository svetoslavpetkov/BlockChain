<template>
<article>
    <div class="pb-4">
        <h1>The ponzi blockchain</h1>
    </div>
      <div class="row">
    <div class="col">
      <h3>Pending transactions</h3>
      <div v-if="pendingTransactions.length > 0">
        <div class="card mt-3 mb-3" v-for="transaction in pendingTransactions" v-bind:key="transaction.transactionHash">
          <div class="card-header ellipsis">
            TX# <router-link :to="{ name: 'transaction', params: { transactionHash: transaction.transactionHash }}">{{ transaction.transactionHash }}</router-link>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-5 ellipsis">
                From <router-link :to="{ name: 'account', params: { address: transaction.fromAddress }}">{{ transaction.fromAddress }}</router-link>
              </div>
              <div class="col-5 ellipsis">
                To <router-link :to="{ name: 'account', params: { address: transaction.toAddress }}">{{ transaction.toAddress }}</router-link>
              </div>
              <div class="col-2 ellipsis">
                <p>Amount {{ transaction.amountString }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    <div v-else>
        <p>No pending transactions</p>
    </div>
    </div>
  </div>
    <div>
      <div class="row">
        <div class="col">
        <h3 class="float-left">Last mined blocks</h3>
           <router-link class="btn float-right" tag="button" :to="{ name: 'blocks'}">View all</router-link>
        </div>
      </div>
      <div class="card mt-3 mb-3" v-for="block in blocks" v-bind:key="block.index">
        <div class="card-header">
          <router-link :to="{ name: 'block', params: { index: block.index }}">Block {{ block.index }}</router-link>
        </div>
        <div class="card-body row">
          <p class="col-6">Mined by <router-link :to="{ name: 'account', params: { address: block.minedBy }}">{{ block.minedBy }}</router-link></p>
          <p class="col-4">{{ block.transactionsCount }} transactions</p>
          <p class="col-2">Block reward {{ block.blockReward }}</p>
        </div>
        <div class="card-footer">
          {{ block.createdDate | dateTime('dd.MM.yyyy hh:mm:ss') }}
        </div>
      </div>
    </div>
  <div class="row">
    <div class="col">
      <div class="">
        <h2>Active peers </h2>
        <div v-if="peers.length > 0">
          <p v-for="peer in peers" v-bind:key="peer.name">
           {{ peer.name }}
            <a :href="getNodeUrl(peer.url)" target="_blank">{{ peer.url }}</a>
          </p>
        </div>
        <div v-else>
         <p>No active peers</p>
        </div>
    </div>
    </div>
  </div>
</article>
</template>

<script>
import apiService from '../services/apiService'
const latestBlocks = 3

export default {
  name: 'Home',
  data () {
    return {
      blocks: [],
      peers: [],
      pendingTransactions: [],
      lastBlock: {},
      totalBlockCount: 0
    }
  },
  methods: {
    getLatestBlocks () {
      apiService.get(this.$http, 'block', 'last', {
        template: '$count$',
        request: {
          count: latestBlocks
        }
      })
        .then(result => {
          if (result.body.length === 0) {
            return
          }
          this.blocks = result.body
          this.totalBlockCount += result.body.length
        }, error => {
          console.log(error)
        })
    },
    getActivePeers () {
      apiService.get(this.$http, 'peers', '')
        .then(result => {
          this.peers = result.body
        }, error => {
          console.log(error)
        })
    },
    getPendingTransactions () {
      apiService.get(this.$http, 'transaction', 'pending')
        .then(result => {
          this.pendingTransactions = result.body
        }, error => {
          console.log(error)
        })
    },
    getNodeUrl(url){
       return `${url}/api/info`
    }
  },
  created () {
    this.getLatestBlocks()
    this.getActivePeers()
    this.getPendingTransactions()
  }
}
</script>
