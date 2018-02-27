import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Transaction from '@/components/Transaction'
import Block from '@/components/Block'
import Account from '@/components/Account'

Vue.use(Router)

export default new Router({
  routes: [
    { path: '/', name: 'home', component: Home },
	{ path: '/transaction/:transactionHash', name: 'transaction', component: Transaction },
    { path: '/block/:index', name: 'block', component: Block },
    { path: '/account/:address', name: 'account', component: Account }
  ]
})
