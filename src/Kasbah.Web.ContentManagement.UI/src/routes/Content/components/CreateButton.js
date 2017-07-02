import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as contentActions } from 'store/appReducers/content'
import NodeForm from 'forms/NodeForm'

class CreateButton extends React.Component {
  static propTypes = {
    parent: PropTypes.string,
    className: PropTypes.string,
    content: PropTypes.object.isRequired,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    createNode: PropTypes.func.isRequired
  }

  handleClick = () => {
    this.props.showModal(null, this.form, null)
  }

  handleSave = (values) => {
    const { parent } = this.props

    this.props.createNode({ parent, ...values })
  }

  get form() {
    return (<NodeForm
      onClose={this.props.hideModal}
      onSubmit={this.handleSave}
      types={this.props.content.types.list} />)
  }

  render() {
    const { parent } = this.props
    return (
      <button className={this.props.className} type='button' onClick={this.handleClick}>
        <span>{parent ? 'New child node' : 'New root level node'}</span>
      </button>
    )
  }
}

const mapStateToProps = (state) => ({
  content: state.content
})

const mapDispatchToProps = {
  ...uiActions,
  ...contentActions
}

export default connect(mapStateToProps, mapDispatchToProps)(CreateButton)
