<template>
    <article>
        <div class="jumbotron">
            <div class="row">
                <div class="col-2">
                    <strong>Address</strong>
                </div>
                <div class="col-10">{{ address }}</div>
            </div>
            <div class="row">
                <div class="col-2">
                    <strong>Balance</strong>
                </div>
                <div class="col-10">{{ balance }}</div>
            </div>
            <div class="mt-3">
                <h4>Transactions ({{ transactions.length }})</h4>
                <div v-if="transactions.length > 0" class="table-responsive mt-2">
                    <table class="table table-striped">
                        <thead>
                            <tr class="d-inline-block col-12 text-truncate p-0">
                                <th class="d-inline-block col-2 text-truncate">TX hash</th>
                                <th class="d-inline-block col-2 text-truncate">Date</th>
                                <th class="d-inline-block col-2 text-truncate">From</th>
                                <th class="d-inline-block col-1 text-truncate">Direction</th>
                                <th class="d-inline-block col-2 text-truncate">To</th>
                                <th class="d-inline-block col-1 text-truncate">Value</th>
                                <th class="d-inline-block col-2 text-truncate">Comment</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="transaction in transactions" :key="transaction.id">
                                <td class="d-inline-block col-2 text-truncate">
                                    <router-link :to="{ name: 'transaction', params: { transactionHash: transaction.transactionHash } }">{{ transaction.transactionHash }}</router-link>
                                </td>
                                <td class="d-inline-block col-2 text-truncate">{{ transaction.dateCreated | dateTime('yyyy.MM.dd hh:mm:ss') }}</td>
                                <td class="d-inline-block col-2 text-truncate">
                                    <router-link :to="{ name: 'account', params: { address: transaction.fromAddress } }">{{ transaction.fromAddress }}</router-link>
                                </td>
                                <td class="d-inline-block col-1 text-truncate">
                                    <span :class="getTransactionDiretionClass(transaction)">{{ getTransactionDirection(transaction) }}</span>
                                </td>
                                <td class="d-inline-block col-2 text-truncate">
                                    <router-link :to="{ name: 'account', params: { address: transaction.toAddress } }">{{ transaction.toAddress }}</router-link>
                                </td>
                                <td class="d-inline-block col-1 text-truncate">{{ transaction.amountString }}</td>
                                <td class="d-inline-block col-2 text-truncate">{{ transaction.comment }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div v-else class="mt-2">
                    No transactions
                </div>
            </div>
            
        </div>
    </article>
</template>
<script>
import apiService from '../services/apiService'

export default {
  data () {
    return {
      address: this.$route.params.address,
      balance: '',
      transactions: []
    }
  },
  methods: {
    getBalance () {
      apiService.get(this.$http, 'account', '', {
        template: '$address$/ballance',
        request: {
          address: this.$route.params.address
        }
      })
        .then(result => {
          this.balance = result.body
        }, error => {
          console.log(error)
        })
    },
    getTransactions () {
      apiService.get(this.$http, 'account', '', {
        template: '$address$/latesttransactions/$count$',
        request: {
          address: this.$route.params.address,
          count: 10 // TODO: Modify this
        }
      })
        .then(result => {
          this.transactions = result.body
        }, error => {
          console.log(error)
        })
    },
    getTransactionDirection (transaction) {
      if (transaction.fromAddress === this.$route.params.address) return 'Out'
      if (transaction.toAddress === this.$route.params.address) return 'In'
    },
    getTransactionDiretionClass (transaction) {
      if (transaction.fromAddress === this.$route.params.address) return 'transaction-out'
      if (transaction.toAddress === this.$route.params.address) return 'transaction-in'
    }
  },
  watch: {
    '$route.params.address': function (value) {
      this.getBalance()
      this.getTransactions()
    }
  },
  created () {
    this.getBalance()
    this.getTransactions()
  }
}
</script>
