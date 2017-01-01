import React from 'react'

export class Tab extends React.Component {
  static propTypes = {
    title: React.PropTypes.string.isRequired,
    children: React.PropTypes.any.isRequired
  }

  render() {
    return this.props.children
  }
}

class TabHeader extends React.Component {
  static propTypes = {
    index: React.PropTypes.number.isRequired,
    currentIndex: React.PropTypes.number.isRequired,
    title: React.PropTypes.string.isRequired,
    onClick: React.PropTypes.func.isRequired
  }

  constructor() {
    super()

    this.handleClick = this.handleClick.bind(this)
  }

  handleClick() {
    this.props.onClick(this.props.index)
  }

  render() {
    const attrs = {
      className: this.props.index === this.props.currentIndex ? 'is-active' : null,
      onClick: this.handleClick
    }

    return (<a {...attrs}>{this.props.title}</a>)
  }
}

export class Tabs extends React.Component {
  static propTypes = {
    children: React.PropTypes.arrayOf(React.PropTypes.node)
  }

  constructor() {
    super()

    this.state = { currentIndex: 0 }
    this.setCurrentIndex = this.setCurrentIndex.bind(this)
  }

  setCurrentIndex(index) {
    this.setState({
      currentIndex: index
    })
  }

  render() {
    return (
      <div className='panel'>
        <p className='panel-tabs'>
          {this.props.children.map((ent, index) => {
            const attrs = {
              key: index,
              title: ent.props.title,
              index: index,
              currentIndex: this.state.currentIndex,
              onClick: this.setCurrentIndex
            }
            return <TabHeader {...attrs} />
          })}
        </p>
        <div className='panel-block'>
          {this.props.children[this.state.currentIndex]}
        </div>
      </div>)
  }
}

export default Tabs
