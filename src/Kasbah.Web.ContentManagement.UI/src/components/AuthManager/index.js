import React from 'react'
import moment from 'moment'
import { connect } from 'react-redux'
import { makeApiRequest } from 'store/util'

// TODO: this doesn't really belong under `components`

export class AuthManager extends React.Component {
  static propTypes = {
    children: React.PropTypes.any
  }

  static contextTypes = {
    router: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = {
      timeout: null
    }

    this.checkRefresh = this.checkRefresh.bind(this)
  }

  componentDidMount() {
    this.checkRefresh()
  }

  componentWillDismount() {
    if (this.state.timeout) {
      clearTimeout(this.state.timeout)
    }
  }

  checkRefresh() {
    // TODO: move this to a reducer
    if (moment().add(5, 'seconds').isAfter(moment(localStorage.accessTokenExpires))) {
      const user = JSON.parse(localStorage.user)
      console.log('issue refresh token request')
      makeApiRequest({
        url: '/connect/token',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded'
        },
        rawBody: `grant_type=refresh_token&refresh_token=${user.refresh_token}`
      })
        .then(res => {
          localStorage.user = JSON.stringify(res)
          localStorage.accessTokenExpires = moment().add(res.expires_in, 'seconds').format()

          this.queueCheckRefresh()
        })
        .catch(err => {
          console.error(err)

          this.context.router.push('/login')
        })
    } else {
      this.queueCheckRefresh()
    }
  }

  queueCheckRefresh() {
    try {
      this.setState({
        timeout: setTimeout(this.checkRefresh, 1000)
      })
    } catch (ex) {
      console.log('probably dismounted')
    }
  }

  render() {
    return this.props.children
  }
}

const mapDispatchToProps = {
}

const mapStateToProps = (state) => ({})

export default connect(mapStateToProps, mapDispatchToProps)(AuthManager)
