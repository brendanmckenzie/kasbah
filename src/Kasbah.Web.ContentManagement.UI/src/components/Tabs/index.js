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
    return (<li {...attrs}><a>{this.props.title}</a></li>)
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

  get tabs() {
    if (this.props.children instanceof Array) {
      if (this.props.children.length === 1) {
        return null
      } else {
        return (
          <div className='tabs'>
            <ul>
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
            </ul>
          </div>
        )
      }
    }

    return null
  }

  render() {
    return (
      <div className='tabs__container'>
        {this.tabs}
        {this.props.children[this.state.currentIndex]}
      </div>)
  }
}

export default Tabs
