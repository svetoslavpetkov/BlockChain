<template>
<article>
  <div class="row">
    <div class="col">
      <div class="jumbotron">
        <h2>Last block <router-link :to="{ name: 'block', params: { index: lastBlock.index } }">{{ lastBlock.index }}</router-link></h2>
        <div class="row">
          <div class="col">
            <p><strong>Created date</strong></p>
            <span>{{ lastBlock.createdDate | dateTime('dd.MM.yyyy hh:mm:ss') }}</span>
            <p><strong>Reward</strong></p>
            <span>{{ lastBlock.blockReward }}</span>
          </div>
          <div class="col">
            <p><strong>Mined by</strong></p>
            <span>{{ lastBlock.minedBy }}</span>
          </div>
        </div>
      </div>
    </div>
    <div class="col">
      <div class="jumbotron">
        <h2>Active peers ({{ peers.length }})</h2>
        <p v-for="peer in peers" v-bind:key="peer.name">
          <span>{{ peer.name }}</span>
          <a :href="peer.url">{{ peer.url }}</a>
        </p>
      </div>
    </div>
  </div>
  <div class="row">
    <div class="col">
      <div>
        <h3>Previous blocks</h3>
        <div class="card mt-3 mb-3" v-for="block in blocks" v-bind:key="block.index">
          <div class="card-header">
            Block {{ block.index }}
          </div>
          <div class="card-body">
            <p>Mined by <router-link :to="{ name: 'account', params: { id: block.minedBy }}">{{ block.minedBy }}</router-link></p>
            <p>{{ block.transactionsCount }} transactions</p>
            <p>Block reward {{ block.blockReward }}</p>
          </div>
          <div class="card-footer">
            {{ block.createdDate | dateTime('dd.MM.yyyy hh:mm:ss') }}
          </div>
        </div>
      </div>
    </div>
    <div class="col">
      <div>
        <h3>Pending transactions</h3>
        <div class="card mt-3 mb-3" v-for="transaction in pendingTransactions" v-bind:key="transaction.transactionHash">
          <div class="card-header">
            TX# <router-link :to="{ name: 'transaction', params: { transactionHash: transaction.transactionHash }}">{{ transaction.transactionHash }}</router-link>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col">
                From <router-link :to="{ name: 'miner', params: { minerId: transaction.fromAddress }}">{{ transaction.fromAddress }}</router-link>
              </div>
              <div class="col">
                To <router-link :to="{ name: 'miner', params: { minerId: transaction.fromAddress }}">{{ transaction.fromAddress }}</router-link>
              </div>
            </div>
            <p>Amount {{ transaction.amountString }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</article>
</template>

<script>
import apiService from '../services/apiService'
const itemsPerPage = 10

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
    getLastBlock () {
      apiService.get(this.$http, 'block', 'last')
        .then(result => {
          this.lastBlock = result.body
          // Get latest blocks here because the call depends on the last block's index
          this.getLatestBlocks()
          this.getActivePeers()
          this.getPendingTransactions()
        }, error => {
          console.log(error)
        })
    },
    getLatestBlocks () {
      var itemsCount = this.lastBlock.index < this.totalBlockCount + itemsPerPage ? this.lastBlock.index - (this.totalBlockCount - 1) : itemsPerPage
      if (this.totalBlockCount > this.lastBlock.index) {
        return
      }
      apiService.get(this.$http, 'block', 'getblocksByFromIndexAndCount', {
        template: '$fromIndex$/$count$',
        request: {
          fromIndex: this.lastBlock.index - 1,
          count: itemsCount
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
    }
  },
  created () {
    this.getLastBlock()
  }
}
</script>
