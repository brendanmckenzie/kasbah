import React from 'react'

class NodePicker extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired
  }

  static alias = 'nodePicker'

  render() {
    // const { input: { value, onChange } } = this.props

    return (<div>I'm a node picker</div>)
  }
}

export default NodePicker
