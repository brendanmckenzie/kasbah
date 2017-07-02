import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import Header from 'components/Header'
import Modal from 'components/Modal'
import Loading from 'components/Loading'
import 'styles/core.scss'

const AwaitingAuth = () => (<Loading message='Checking credentials' />)

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
