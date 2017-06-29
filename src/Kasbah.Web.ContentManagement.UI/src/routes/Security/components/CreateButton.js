import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as securityActions } from 'store/appReducers/security'
import UserForm from 'forms/UserForm'

class CreateButton extends React.Component {
  static propTypes = {
    className: PropTypes.string,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    createUser: PropTypes.func.isRequired
  }

  handleClick = () => {
    this.props.showModal(null, this.form, null)
  }

  handleSave = (values) => {
    this.props.createUser(values)
  }

  get form() {
    return (<UserForm
      onClose={this.props.hideModal}
      onSubmit={this.handleSave} />)
  }

  render() {
    return (
      <button className={this.props.className} type='button' onClick={this.handleClick}>
        <span>New user</span>
      </button>
    )
  }
}

const mapDispatchToProps = {
  ...uiActions,
  ...securityActions
}

export default connect(null, mapDispatchToProps)(CreateButton)
