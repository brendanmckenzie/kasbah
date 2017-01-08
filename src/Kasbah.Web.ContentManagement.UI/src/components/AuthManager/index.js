import React from 'react'
import moment from 'moment'
import { connect } from 'react-redux'

export class AuthManager extends React.Component {
  static propTypes = {
    children: React.PropTypes.node
  }

  componentDidMount() {
    setTimeout(() => {
      if (moment().add(5, 'minutes').isAfter(moment(localStorage.accessTokenExpires))) {
        // TODO: refresh access token
        console.log('issue refresh token request')
      }
    }, 1000)
  }

  render() {
    return this.props.children
  }
}

const mapDispatchToProps = {
}

const mapStateToProps = (state) => ({})

export default connect(mapStateToProps, mapDispatchToProps)(AuthManager)
