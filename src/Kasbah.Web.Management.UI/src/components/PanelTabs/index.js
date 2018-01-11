import React from 'react'
import PropTypes from 'prop-types'

export class Tab extends React.Component {
  static propTypes = {
    children: PropTypes.any.isRequired
  }

  render() {
    return this.props.children
  }
}

class TabHeader extends React.Component {
  static propTypes = {
    index: PropTypes.number.isRequired,
    currentIndex: PropTypes.number.isRequired,
    title: PropTypes.string.isRequired,
    onClick: PropTypes.func.isRequired
  }

  handleClick = () => this.props.onClick(this.props.index)

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
    children: PropTypes.arrayOf(PropTypes.node)
  }

  constructor() {
    super()

    this.state = { currentIndex: 0 }
  }

  setCurrentIndex = (currentIndex) => this.setState({ currentIndex })

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
