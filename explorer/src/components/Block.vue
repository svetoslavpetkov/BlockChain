<template>
  <article>
    <div class="jumbotron">
      <h2>Block #{{ block.index }}</h2>
      <div class="row">
        <div class="col-4">
          <p>Index</p>
        </div>
        <div class="col-8">
          <p>{{ block.index }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Created date</p>
        </div>
        <div class="col-8">
          <p>{{ block.createdDate | dateTime('yyyy.MM.dd hh:mm:ss') }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Previous block hash</p>
        </div>
        <div class="col-8 ellipsis">
          <p>{{ block.previousBlockHash }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Data hash</p>
        </div>
        <div class="col-8 ellipsis">
          <p>{{ block.blockDataHash }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Reward</p>
        </div>
        <div class="col-8">
          <p>{{ block.blockReward }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Difficulty</p>
        </div>
        <div class="col-8">
          <p>{{ block.difficulty }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Nonce</p>
        </div>
        <div class="col-8">
          <p>{{ block.nonce }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Hash</p>
        </div>
        <div class="col-8 ellipsis">
          <p>{{ block.blockHash }}</p>
        </div>
      </div>
      <div class="row">
        <div class="col-4">
          <p>Mined by</p>
        </div>
        <div class="col-8 ellipsis">
          <p>
            <router-link :to="{ name: 'account', params: { address: block.minedBy }}">{{ block.minedBy }}</router-link>
          </p>
        </div>
      </div>
    </div>
    <div class="jumbotron">
      <h2>Transactions ({{ transactions.length }})</h2>
      <div v-if="transactions.length > 0" class="table-responsive mt-2">
        <table class="table table-striped">
          <thead>
            <tr class="d-inline-block col-12 text-truncate">
              <th class="d-inline-block col-4 text-truncate">TX hash</th>
              <th class="d-inline-block col-2 text-truncate">Date</th>
              <th class="d-inline-block col-2 text-truncate">From</th>
              <th class="d-inline-block col-2 text-truncate">To</th>
              <th class="d-inline-block col-2 text-truncate">Value</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="transaction in transactions" :key="transaction.id">
              <td class="d-inline-block col-4 text-truncate">
                <router-link :to="{ name: 'transaction', params: { transactionHash: transaction.transactionHash } }">{{ transaction.transactionHash }}</router-link>
              </td>
              <td class="d-inline-block col-2 text-truncate">{{ transaction.dateCreated | dateTime('yyyy.MM.dd hh:mm:ss') }}</td>
              <td class="d-inline-block col-2 text-truncate">
                <router-link :to="{ name: 'account', params: { address: transaction.fromAddress } }">{{ transaction.fromAddress }}</router-link>
              </td>
              <td class="d-inline-block col-2 text-truncate">
                <router-link :to="{ name: 'account', params: { address: transaction.toAddress } }">{{ transaction.toAddress }}</router-link>
              </td>
              <td class="d-inline-block col-2 text-truncate">{{ transaction.amountString }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else class="mt-2">
        No transactions
      </div>
    </div>
  </article>
</template>
<script>
import apiService from '../services/apiService'

export default {
  data () {
    return {
      block: {},
      transactions: []
    }
  },
  methods: {
    getBlock () {
      apiService.get(this.$http, 'block', '', {
        template: '$index$',
        request: {
          index: this.$route.params.index
        }
      })
        .then(result => {
          this.block = result.body
        }, error => {
          console.log(error)
        })
    },
    getBlockTransactions () {
      apiService.get(this.$http, 'block', '', {
        template: '$index$/transactions',
        request: {
          index: this.$route.params.index
        }
      })
        .then(result => {
          this.transactions = result.body
        }, error => {
          console.log(error)
        })
    }
  },
  watch: {
    // It is possible to go to the same view with different parameter which
    // does not recompile the view so call get functions explicitly
    '$route.params.index': function (index) {
      this.getBlock()
      this.getBlockTransactions()
    }
  },
  created () {
    this.getBlock()
    this.getBlockTransactions()
  }
}
</script>
