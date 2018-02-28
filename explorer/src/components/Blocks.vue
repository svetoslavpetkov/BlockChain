<template>
  <article>
        <div>
            <h3>{{blocks.length}} blocks total</h3>        
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
  </article>
</template>

<script>
import apiService from '../services/apiService'
export default {
    data () {
        return {
            blocks: []
        }
    },
    methods: {
        getAllBlocks() {
            apiService.get(this.$http, 'block', 'all')
            .then(result => {
                if (result.body.length === 0) {
                    return
                }
                this.blocks = result.body
                }, error => {
                 console.log(error)
            })
        }
    },
    created(){
       this.getAllBlocks()
    }
}
</script>
