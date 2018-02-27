<template>
  <article>
      <div class="jumbotron">
        <h2 class="text-truncate">Transaction {{ $route.params.transactionHash }}</h2>
        <div class="row">
          <div class="col-4">
            <p>Block index</p>
          </div>
          <div class="col-8">
            <router-link :to="{ name: 'block', params: { index: transaction.minedInBlock }}">{{ transaction.minedInBlock }}</router-link>
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Status</p>
          </div>
          <div class="col-8" :class="getTransactionStatusClass(transaction.status)">
            <p>{{ getTransactionStatusText(transaction.status) }}</p>
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Created date</p>
          </div>
          <div class="col-8">
            {{ transaction.dateCreated | dateTime('yyyy.MM.dd hh:mm:ss') }}
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>From</p>
          </div>
          <div class="col-8 ellipsis">
            <router-link :to="{ name: 'account', params: { address: transaction.fromAddress }}">{{ transaction.fromAddress }}</router-link>
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>To</p>
          </div>
          <div class="col-8 ellipsis">
            <router-link :to="{ name: 'account', params: { address: transaction.toAddress }}">{{ transaction.toAddress }}</router-link>
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Value</p>
          </div>
          <div class="col-8">
            {{ transaction.amountString }}
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Fee</p>
          </div>
          <div class="col-8">
            {{ transaction.feeString }}
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Signature</p>
          </div>
          <div class="col-8 ellipsis">
            {{ getTransactionSignature(transaction.signature) }}
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Comment</p>
          </div>
          <div class="col-8 ellipsis">
            {{ transaction.comment }}
          </div>
        </div>
        <div class="row">
          <div class="col-4">
            <p>Hash</p>
          </div>
          <div class="col-8 ellipsis">
            {{ transaction.transactionHash }}
          </div>
        </div>
      </div>
  </article>
</template>
<script>
import apiService from '../services/apiService'

const transactionStatuses = {
  pending: 0,
  declined: 1,
  approved: 2
}

export default {
  data () {
    return {
      transaction: {}
    }
  },
  methods: {
    getTransaction () {
      apiService.get(this.$http, 'transaction', '', {
        template: '$transactionHash$',
        request: {
          transactionHash: this.$route.params.transactionHash
        }
      })
        .then(result => {
          this.transaction = result.body
        }, error => {
          console.log(error)
        })
    },
    getTransactionStatusClass (status) {
      switch (status) {
        case transactionStatuses.pending: return 'transaction-pending'
        case transactionStatuses.declined: return 'transaction-declined'
        case transactionStatuses.approved: return 'transaction-approved'
      }
    },
    getTransactionStatusText (status) {
      switch (status) {
        case transactionStatuses.pending: return 'Pending'
        case transactionStatuses.declined: return 'Declined'
        case transactionStatuses.approved: return 'Approved'
      }
    },
    getTransactionSignature (signature) {
      if (signature && signature.length > 0) {
        return signature.join(', ')
      }
    }
  },
  created () {
    this.getTransaction()
  }
}
</script>
