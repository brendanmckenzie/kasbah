import React from 'react'
import PropTypes from 'prop-types'
import CreateNodeForm from 'forms/CreateNodeForm'

class CreateNode extends React.Component {
  static propTypes = {
    visible: PropTypes.bool,
    parent: PropTypes.string,
    onCancel: PropTypes.func,
    createNodeRequest: PropTypes.func,
    createNode: PropTypes.object,
    listTypesRequest: PropTypes.func,
    listTypes: PropTypes.object
  }

  componentWillMount() {
    if (this.props.listTypesRequest) {
      this.props.listTypesRequest()
    }
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.createNode.success && nextProps.createNode.success !== this.props.createNode.success) {
      this.props.onCancel()
    }
  }

  handleSubmit = (values) => {
    const data = {
      parent: this.props.parent,
      ...values
    }
    this.props.createNodeRequest(data)
  }

  render() {
    if (!this.props.visible) { return null }
    return (
      <CreateNodeForm
        onSubmit={this.handleSubmit}
        onClose={this.props.onCancel}
        types={this.props.listTypes.payload}
        loading={this.props.createNode.loading} />
    )
  }
}

export default CreateNode
