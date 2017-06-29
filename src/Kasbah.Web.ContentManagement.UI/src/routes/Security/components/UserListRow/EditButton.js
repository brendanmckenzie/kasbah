import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as securityActions } from 'store/appReducers/security'
import UserForm from 'forms/UserForm'

class EditButton extends React.Component {
  static propTypes = {
    user: PropTypes.object.isRequired,
    className: PropTypes.string,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    putUser: PropTypes.func.isRequired
  }

  handleClick = () => {
    this.props.showModal(null, this.form, null)
  }

  handleSave = (values) => {
    this.props.putUser(values)
  }

  get form() {
    return (<UserForm
      initialValues={this.props.user}
      onClose={this.props.hideModal}
      onSubmit={this.handleSave} />)
  }

  render() {
    return (
      <button className={this.props.className} type='button' onClick={this.handleClick}>
        <span className='icon is-small'><i className='fa fa-pencil' /></span>
        <span>Edit</span>
      </button>
    )
  }
}

const mapDispatchToProps = {
  ...uiActions,
  ...securityActions
}

export default connect(null, mapDispatchToProps)(EditButton)
