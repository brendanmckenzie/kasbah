import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import LoginForm from '../forms/LoginForm'

export class View extends React.Component {
  static propTypes = {
    loginRequest: PropTypes.func.isRequired,
    login: PropTypes.object.isRequired
  }

  static contextTypes = {
    router: PropTypes.object.isRequired
  }

  componentWillMount() {
    delete localStorage.user
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.login.success && nextProps.login.success !== this.props.login.success) {
      localStorage.user = JSON.stringify(nextProps.login.payload)
      localStorage.accessTokenExpires = moment().add(nextProps.login.payload.expires_in, 'seconds').format()

      this.context.router.push('/')
    }
  }

  render() {
    return (
      <div className='hero is-fullheight'>
        <div className='hero-body'>
          <div className='container'>
            <div className='columns'>
              <div className='column is-4 is-offset-4'>
                <div className='notification is-primary'>
                  <p className='heading'>Content authoring and marketing platform</p>
                  <div className='level'>
                    <div className='level-left'>
                      <p className='title'>Kasbah</p>
                    </div>
                    <div className='level-right'>
                      <small>v1.0.0.0</small>
                    </div>
                  </div>
                </div>
                <LoginForm onSubmit={this.props.loginRequest} {...this.props.login} />
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default View
