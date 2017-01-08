import React from 'react'

class MediaPicker extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired
  }

  static alias = 'mediaPicker'

  render() {
    // const { input: { value, onChange } } = this.props

    return (<div>I'm a media picker</div>)
  }
}

export default MediaPicker
