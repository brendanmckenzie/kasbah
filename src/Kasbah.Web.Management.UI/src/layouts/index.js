import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import Header from 'components/Header'
import Modal from 'components/Modal'
import Loading from 'components/Loading'
import { Container, Columns, Column } from 'components/Layout'
import 'styles/core.scss'

const AwaitingAuth = () => (<div className='hero is-fullheight'>
  <div className='hero-body'>
    <Container>
      <Columns>
        <Column className='is-4 is-offset-4'>
          <Loading message='Checking credentials' />
        </Column>
      </Columns>
    </Container>
  </div>
</div>)

export const CoreLayout = ({ children, ui, auth }) => auth.ready ? (
  <div>
    <Header />
    {children}
    <Modal modal={ui.modal} />
  </div>
) : <AwaitingAuth />

CoreLayout.propTypes = {
  children: PropTypes.any.isRequired,
  ui: PropTypes.object.isRequired,
  auth: PropTypes.object.isRequired
}

const mapStateToProps = (state) => ({
  ui: state.ui,
  auth: state.auth
})

export default withRouter(connect(mapStateToProps)(CoreLayout))
