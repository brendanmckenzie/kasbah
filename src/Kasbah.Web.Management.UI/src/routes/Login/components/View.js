import React from 'react'
import PropTypes from 'prop-types'
import LoginForm from 'forms/LoginForm'
import { Container, Columns, Column } from 'components/Layout'

const View = ({ login, auth }) => (
  <div className='hero is-fullheight'>
    <div className='hero-body'>
      <Container>
        <Columns>
          <Column className='is-4 is-offset-4'>
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
            <LoginForm onSubmit={login} loading={auth.authenticating} errorMessage={auth.error} />
          </Column>
        </Columns>
      </Container>
    </div>
  </div>
)

View.propTypes = {
  login: PropTypes.func.isRequired,
  auth: PropTypes.object.isRequired
}

export default View
